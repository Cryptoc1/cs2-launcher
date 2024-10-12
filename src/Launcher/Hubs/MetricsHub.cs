using System.Timers;
using CS2Launcher.AspNetCore.App.Abstractions.Api;
using CS2Launcher.AspNetCore.App.Abstractions.Signals;
using CS2Launcher.AspNetCore.Launcher.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Timer = System.Timers.Timer;

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
    /// <summary> Subscribes the given <paramref name="connectionId"/> to <see cref="MetricsSignals.Report"/>. </summary>
    IAsyncDisposable Subscribe( string connectionId );
}

internal sealed class MetricsHubSubscriber( IHubContext<MetricsHub> metricsHub, IDedicatedServer server ) : IAsyncDisposable, IMetricsHubSubscriber
{
    private readonly MetricsMonitor monitor = new( server );

    public ValueTask DisposeAsync( ) => monitor.DisposeAsync();

    public IAsyncDisposable Subscribe( string connectionId )
    {
        ArgumentException.ThrowIfNullOrEmpty( connectionId );
        return new MetricsSubscription( connectionId, metricsHub, monitor );
    }

    private sealed class MetricsMonitor : IAsyncDisposable
    {
        private readonly CancellationTokenSource cancellation = new();
        private readonly Timer timer;

        public TimeSpan SampleRate { get; init; } = TimeSpan.FromSeconds( 3 );

        public MetricsMonitor( IDedicatedServer server )
        {
            timer = new( SampleRate )
            {
                AutoReset = true,
                Enabled = false,
            };

            timer.Elapsed += OnTimerElapsed(
                server,
                ( status, metrics, cancellation ) => OnTickCore?.Invoke( status, metrics, cancellation ) ?? Task.CompletedTask,
                cancellation.Token );
        }

        private int count;
        private event MetricsMonitorHandler? OnTickCore;
        public event MetricsMonitorHandler OnTick
        {
            add
            {
                OnTickCore += value;
                if( Interlocked.Increment( ref count ) > 0 && !timer.Enabled )
                {
                    // NOTE: start the underlying timer if we have listeners
                    timer.Start();
                }
            }
            remove
            {
                OnTickCore -= value;
                if( Interlocked.Decrement( ref count ) <= 0 && timer.Enabled )
                {
                    // NOTE: stop the underlying timer if we don't have listeners
                    timer.Stop();
                }
            }
        }

        public async ValueTask DisposeAsync( )
        {
            if( timer.Enabled )
            {
                timer.Stop();
            }

            await cancellation.CancelAsync();
            cancellation.Dispose();

            timer.Dispose();
        }

        private static ElapsedEventHandler OnTimerElapsed( IDedicatedServer server, MetricsMonitorHandler handler, CancellationToken cancellation )
        {
            ArgumentNullException.ThrowIfNull( server );
            ArgumentNullException.ThrowIfNull( handler );
            return async ( _, _ ) =>
            {
                var status = await server.Status( cancellation );
                var metrics = status switch
                {
                    ServerStatus.Starting or ServerStatus.Running => await server.Metrics( cancellation ),
                    _ => ServerMetrics.Zero,
                };

                await handler.Invoke( status, metrics, cancellation );
            };
        }
    }

    private delegate Task MetricsMonitorHandler( ServerStatus status, ServerMetrics metrics, CancellationToken cancellation = default );

    private sealed class MetricsSubscription : IAsyncDisposable
    {
        private readonly CancellationTokenSource cancellation = new();
        private readonly IHubContext<MetricsHub> metricsHub;
        private readonly MetricsMonitor monitor;

        public string ConnectionId { get; }

        public MetricsSubscription( string connectionId, IHubContext<MetricsHub> metricsHub, MetricsMonitor monitor )
        {
            ArgumentException.ThrowIfNullOrEmpty( connectionId );
            ConnectionId = connectionId;

            this.metricsHub = metricsHub;
            this.monitor = monitor;

            monitor.OnTick += OnMonitorTicked;
        }

        public async ValueTask DisposeAsync( )
        {
            monitor.OnTick -= OnMonitorTicked;

            await cancellation.CancelAsync();
            cancellation.Dispose();
        }

        private async Task OnMonitorTicked( ServerStatus status, ServerMetrics metrics, CancellationToken cancellation )
        {
            using var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(
                this.cancellation.Token,
                cancellation );

            await metricsHub.Clients.Client( ConnectionId )
                .Signal<MetricsSignals.Report>( new( metrics, status ), cancellationSource.Token );
        }
    }
}