using System.ComponentModel.DataAnnotations;
using CS2Launcher.AspNetCore.App.Abstractions.Signals;
using CS2Launcher.AspNetCore.App.Components;
using CS2Launcher.AspNetCore.App.Infrastructure;
using CS2Launcher.AspNetCore.App.Interop;

namespace CS2Launcher.AspNetCore.App.Pages;

public sealed record ConsoleState : State
{
    private const string HistoryStorageKey = "console.history";
    private const int MaxEntries = 250;

    public string? ConnectError { get; init; }
    public Dictionary<long, CommandEntry> Entries { get; init; } = new Dictionary<long, CommandEntry>( MaxEntries );
    public ConsoleForm Form { get; init; } = new();
    public List<string> History { get; set; } = [];
    public string Host { get; init; }
    public bool IsLoading { get; init; } = true;

    internal static async IAsyncEnumerable<ConsoleState> ExecuteCommand( ConsoleSignaler signaler, ConsoleState state )
    {
        ArgumentNullException.ThrowIfNull( signaler );
        ArgumentNullException.ThrowIfNull( state );

        var entries = new Dictionary<long, CommandEntry>( state.Entries );
        if( entries.Count >= MaxEntries )
        {
            var token = entries.Keys.First();
            entries.Remove( token );
        }

        var signal = new ConsoleSignals.ExecuteCommand( state.Form.Command, DateTime.UtcNow.Ticks );
        entries.Add( signal.Token, new( signal.Command ) );

        var history = new List<string>( state.History );
        if( history.Count >= MaxEntries )
        {
            history.RemoveAt( history.Count - 1 );
        }
        else if( history.ElementAtOrDefault( history.Count - 1 ) != signal.Command )
        {
            history.Add( signal.Command );
        }

        yield return state with
        {
            Entries = entries,
            Form = new(),
            History = history,
        };

        await signaler.Send( signal );
    }

    internal static async IAsyncEnumerable<ConsoleState> Load( LocalStorageInterop localStorage, ConsoleSignaler signaler, ConsoleState state )
    {
        ArgumentNullException.ThrowIfNull( localStorage );
        ArgumentNullException.ThrowIfNull( state );

        var connect = signaler.Connect();
        yield return state with
        {
            History = await LoadHistory( localStorage )
        };

        await connect;

        static async ValueTask<List<string>> LoadHistory( LocalStorageInterop localStorage )
        {
            var history = await localStorage.Get<List<string>>( HistoryStorageKey ) ?? [];
            history.EnsureCapacity( MaxEntries );

            return history;
        }
    }

    internal static ConsoleState OnConnectFailed( ConsoleSignals.ConnectFailed failure, ConsoleState state )
        => state with
        {
            ConnectError = failure.Error,
            IsLoading = false,
        };

    internal static ConsoleState OnConnected( ConsoleSignals.Connected connected, ConsoleState state )
        => state with
        {
            ConnectError = "",
            Host = connected.Host,
            IsLoading = false,
        };

    internal static async IAsyncEnumerable<ConsoleState> OnExecutedCommand( LocalStorageInterop localStorage, ConsoleSignals.ExecutedCommand command, ConsoleState state )
    {
        ArgumentNullException.ThrowIfNull( localStorage );
        ArgumentNullException.ThrowIfNull( command );
        ArgumentNullException.ThrowIfNull( state );

        if( state.Entries.TryGetValue( command.Token, out var entry ) )
        {
            yield return state with
            {
                Entries = new Dictionary<long, CommandEntry>( state.Entries )
                {
                    [ command.Token ] = entry with
                    {
                        IsError = command.Text.StartsWith( "unknown command", StringComparison.OrdinalIgnoreCase ),
                        IsExecuting = false,
                        Text = command.Text
                    }
                }
            };

            await localStorage.Set( HistoryStorageKey, state.History );
        }
    }

    internal static ConsoleState OnExecuteCommandFailed( ConsoleSignals.ExecuteCommandFailed failure, ConsoleState state )
    {
        ArgumentNullException.ThrowIfNull( failure );
        ArgumentNullException.ThrowIfNull( state );

        if( state.Entries.TryGetValue( failure.Token, out var entry ) )
        {
            return state with
            {
                Entries = new Dictionary<long, CommandEntry>( state.Entries )
                {
                    [ failure.Token ] = entry with
                    {
                        IsExecuting = false,
                        IsError = true,
                        Text = failure.Error,
                    }
                }
            };
        }

        return state;
    }

    internal static async IAsyncEnumerable<ConsoleState> Reconnect( ConsoleSignaler signaler, ConsoleState state )
    {
        ArgumentNullException.ThrowIfNull( signaler );
        ArgumentNullException.ThrowIfNull( state );

        await signaler.Disconnect();
        yield return state with
        {
            ConnectError = "",
            IsLoading = true,
        };

        await signaler.Connect();
    }
}

public sealed record CommandEntry( string Command, bool IsExecuting = true, string? Text = default )
{
    public bool IsError { get; init; }
}

public sealed record class ConsoleForm
{
    [Required]
    public string Command { get; set; }

    public int Offset { get; set; }
}