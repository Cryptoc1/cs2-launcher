using CS2Launcher.AspNetCore.App.Abstractions.Api;
using CS2Launcher.AspNetCore.App.Abstractions.Signals;
using CS2Launcher.AspNetCore.Launcher.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CS2Launcher.AspNetCore.Launcher.Hubs;

/// <summary> Hub for handling <see cref="MetricsSignals"/>. </summary>
[Authorize]
public sealed class MetricsHub( IMetricsHubSubscriber subscriber ) : Hub
{
    private static readonly object SubscriptionKey = new();

    /// <inheritdoc/>
    public override async Task OnConnectedAsync( )
    {
        await TryDestroySubscription();
        Context.Items[ SubscriptionKey ] = subscriber.Subscribe( Context.ConnectionId );
    }

    /// <inheritdoc/>
    public override async Task OnDisconnectedAsync( Exception? exception ) => await TryDestroySubscription();

    private async ValueTask<bool> TryDestroySubscription( )
    {
        if( Context.Items.Remove( SubscriptionKey, out var value ) && value is IAsyncDisposable subscription )
        {
            await subscription.DisposeAsync();
            return true;
        }

        return false;
    }
}

/// <summary> Describes a service that subscribes connections to server metrics. </summary>
public interface IMetricsHubSubscriber
{
    /// <summary> Subscribes the given <paramref name="connectionId"/> to <see cref="App.Abstractions.Api.ServerMetrics"/>. </summary>
    IAsyncDisposable Subscribe( string connectionId );
}

internal sealed class MetricsHubSubscriber( IHubContext<MetricsHub> metricsHub, IDedicatedServer server ) : IMetricsHubSubscriber
{
    public IAsyncDisposable Subscribe( string connectionId )
    {
        ArgumentException.ThrowIfNullOrEmpty( connectionId );
        return new MetricsSubscription( connectionId, metricsHub, server );
    }

    private sealed class MetricsSubscription : IAsyncDisposable
    {
        private readonly CancellationTokenSource cancellation = new();
        private readonly IHubContext<MetricsHub> metricsHub;
        private readonly IDedicatedServer server;
        private readonly Timer timer;

        public string ConnectionId { get; }

        public MetricsSubscription(
            string connectionId,
            IHubContext<MetricsHub> metricsHub,
            IDedicatedServer server )
        {
            ConnectionId = connectionId;

            this.metricsHub = metricsHub;
            this.server = server;
            timer = new( OnTimerElapsed, this, TimeSpan.Zero, TimeSpan.FromSeconds( 5 ) );
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync( )
        {
            await cancellation.CancelAsync();
            cancellation.Dispose();

            await timer.DisposeAsync();
        }

        private static async void OnTimerElapsed( object? state )
        {
            ArgumentNullException.ThrowIfNull( state );
            if( state is not MetricsSubscription subscription )
            {
                throw new ArgumentException( $"Expected argument of type '{nameof( MetricsSubscription )}'.", nameof( state ) );
            }

            var status = await subscription.server.Status( subscription.cancellation.Token );
            var metrics = status switch
            {
                ServerStatus.Starting or ServerStatus.Running => await subscription.server.Metrics( subscription.cancellation.Token ),
                _ => ServerMetrics.Zero,
            };

            await subscription.metricsHub.Clients.Client( subscription.ConnectionId )
                .Signal<MetricsSignals.Report>( new( metrics, status ), subscription.cancellation.Token );
        }
    }
}