using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using CoreRCON;
using CoreRCON.Extensions.CounterStrike;
using CS2Launcher.AspNetCore.Launcher.Abstractions;
using CoreRCON.Parsers.Standard;
using CS2Launcher.AspNetCore.App.Abstractions.Signals;

namespace CS2Launcher.AspNetCore.Launcher.Hubs;

/// <summary> Hub for handling <see cref="ConsoleSignals"/>. </summary>
[Authorize]
public sealed class ConsoleHub( IServerConsoleFactory consoleFactory ) : Hub
{
    private static readonly object ConsoleKey = new();

    /// <summary> Handle the <see cref="ConsoleSignals.ExecuteCommand"/>. </summary>
    [HubMethodName( nameof( ConsoleSignals.ExecuteCommand ) )]
    public async Task ExecuteCommand( ConsoleSignals.ExecuteCommand command )
    {
        string value;
        try
        {
            value = await GetOrCreateConsole().SendCommandAsync( command.Command, Context.ConnectionAborted );
        }
        catch( Exception exception ) when( exception is RCONException )
        {
            await Clients.Caller.Signal(
                new ConsoleSignals.ExecuteCommandFailed( exception.Message, command.Token ),
                Context.ConnectionAborted );

            return;
        }

        await Clients.Caller.Signal(
            new ConsoleSignals.ExecutedCommand( value, command.Token ),
            Context.ConnectionAborted );
    }

    private RCONClient GetOrCreateConsole( )
    {
        if( Context.Items.TryGetValue( ConsoleKey, out var value ) && value is RCONClient client )
        {
            return client;
        }

        Context.Items[ ConsoleKey ] = client = consoleFactory.Create();
        return client;
    }

    /// <inheritdoc/>
    public override async Task OnConnectedAsync( )
    {
        Status? status = default;
        try
        {
            status = await Connect(
                GetOrCreateConsole(),
                Context.ConnectionAborted );
        }
        catch( RCONException exception )
        {
            TryDestroyConsole();
            await Clients.Caller.Signal(
                new ConsoleSignals.ConnectFailed( exception.Message ),
                Context.ConnectionAborted );
        }

        if( status is not null )
        {
            await Clients.Caller.Signal(
                new ConsoleSignals.Connected( status.Hostname ?? $"{status.Endpoints?.Public}" ),
                Context.ConnectionAborted );
        }

        static async Task<Status> Connect( RCONClient console, CancellationToken cancellation )
        {
            await console.ConnectAsync().ConfigureAwait( false );
            return await console.Status( cancellation ).ConfigureAwait( false );
        }
    }

    /// <inheritdoc/>
    public override Task OnDisconnectedAsync( Exception? exception )
    {
        TryDestroyConsole();
        return base.OnDisconnectedAsync( exception );
    }

    private bool TryDestroyConsole( )
    {
        if( Context.Items.Remove( ConsoleKey, out var value ) && value is RCONClient console )
        {
            console.Dispose();
            return true;
        }

        return false;
    }
}

internal static class HubClientExtensions
{
    public static Task Signal<TSignal>( this IClientProxy client, TSignal signal, CancellationToken cancellation = default )
        where TSignal : Signal<TSignal>
    {
        ArgumentNullException.ThrowIfNull( client );
        return client.SendAsync( typeof( TSignal ).Name, signal, cancellationToken: cancellation );
    }
}