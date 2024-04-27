using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using CoreRCON;
using CoreRCON.Extensions.CounterStrike;
using CS2Launcher.AspNetCore.App.Abstractions.Signaling;
using CS2Launcher.AspNetCore.App.Abstractions;
using Microsoft.Extensions.Options;
using CS2Launcher.AspNetCore.Launcher.Proc;

namespace CS2Launcher.AspNetCore.Launcher.Api;

/// <summary> Hub for handling <see cref="ConsoleSignals"/>. </summary>
[Authorize]
public sealed class ConsoleHub( IOptions<DedicatedServerOptions> serverOptionsAccessor ) : Hub
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
        catch( RCONException exception )
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

        var options = serverOptionsAccessor.Value;
        client = new RCONClient(
            new IPEndPoint( IPAddress.Parse( options.Host ), 27015 ),
            options.RconPassword!,
            new() { AutoConnect = true } );

        Context.Items[ ConsoleKey ] = client;
        return client;
    }

    /// <inheritdoc/>
    public override async Task OnConnectedAsync( )
    {
        await Connected(
            Clients.Caller,
            GetOrCreateConsole(),
            Context.ConnectionAborted );

        await base.OnConnectedAsync();

        static async Task Connected( IClientProxy caller, RCONClient console, CancellationToken cancellation )
        {
            await console.ConnectAsync();

            var status = await console.Status( cancellation );
            await caller.Signal( new ConsoleSignals.Connected( status.Hostname ?? status.Endpoints?.Public.ToString() ?? "" ), cancellation );
        }
    }

    /// <inheritdoc/>
    public override Task OnDisconnectedAsync( Exception? exception )
    {
        if( Context.Items.Remove( ConsoleKey, out var value ) && value is RCONClient console )
        {
            console.Dispose();
        }

        return base.OnDisconnectedAsync( exception );
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