using CS2Launcher.AspNetCore.App.Abstractions.Signals;
using CS2Launcher.AspNetCore.Launcher.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CS2Launcher.AspNetCore.Launcher.Hubs;

/// <summary> Hub for handling <see cref="MetricsSignals"/>. </summary>
[Authorize]
public sealed class MetricsHub( IDedicatedServer server ) : Hub
{
    private static readonly object SubscriptionKey = new();

    /// <inheritdoc/>
    public override async Task OnConnectedAsync( )
    {
        await TryDestroySubscription();
        Context.Items[ SubscriptionKey ] = new MetricsSubscription( Clients.Caller, server );
    }

    /// <inheritdoc/>
    public override async Task OnDisconnectedAsync( Exception? exception ) => await TryDestroySubscription();

    private async ValueTask<bool> TryDestroySubscription( )
    {
        if( Context.Items.Remove( SubscriptionKey, out var value ) && value is MetricsSubscription subscription )
        {
            await subscription.DisposeAsync();
            return true;
        }

        return false;
    }
}

sealed file class MetricsSubscription( IClientProxy client, IDedicatedServer server ) : IAsyncDisposable
{
    private readonly Timer timer = new( OnTimerElapsed, (client, server), TimeSpan.Zero, TimeSpan.FromSeconds( 15 ) );

    public ValueTask DisposeAsync( ) => timer.DisposeAsync();

    private static async void OnTimerElapsed( object? state )
    {
        ArgumentNullException.ThrowIfNull( state );
        if( state is not (IClientProxy client, IDedicatedServer server ) )
        {
            throw new ArgumentException( "The given value is not valid.", nameof( state ) );
        }

        await client.Signal<MetricsSignals.Report>( new( server.GetMetrics() ) );
    }
}