using System.Runtime.CompilerServices;
using CS2Launcher.AspNetCore.App.Abstractions.Api;
using CS2Launcher.AspNetCore.App.Abstractions.Signals;
using CS2Launcher.AspNetCore.App.Infrastructure;
using ESCd.AspNetCore.Components.Stateful;

namespace CS2Launcher.AspNetCore.App.Pages;

public sealed record ProcessState : State<ProcessState>
{
    public bool IsLoading { get; init; } = true;
    public bool IsServerResetting { get; init; }
    public ServerMetrics Metrics { get; init; } = ServerMetrics.Zero;
    public ServerStatus Status { get; init; }

    internal static async IAsyncEnumerable<ProcessState> Load( IServerApi serverApi, MetricsSignaler signaler, ProcessState state, [EnumeratorCancellation] CancellationToken cancellation )
    {
        ArgumentNullException.ThrowIfNull( serverApi );
        ArgumentNullException.ThrowIfNull( state );

        var connect = signaler.Connect( cancellation );
        yield return await Load( serverApi, state, cancellation );

        await connect;

        static async Task<ProcessState> Load( IServerApi serverApi, ProcessState state, CancellationToken cancellation )
        {
            var metrics = serverApi.Metrics( cancellation );
            var status = serverApi.Status( cancellation );

            await Task.WhenAll( metrics, status );
            return state with
            {
                IsLoading = false,
                Metrics = metrics.Result,
                Status = status.Result,
            };
        }
    }

    internal static ProcessState OnReport( MetricsSignals.Report report, ProcessState state )
        => state with
        {
            Metrics = report.Metrics,
            Status = report.Status,
        };

    internal static async IAsyncEnumerable<ProcessState> RestartServer( IServerApi serverApi, ProcessState state, [EnumeratorCancellation] CancellationToken cancellation )
    {
        ArgumentNullException.ThrowIfNull( serverApi );
        ArgumentNullException.ThrowIfNull( state );

        yield return state with
        {
            IsServerResetting = true
        };

        yield return state with
        {
            IsServerResetting = false,
            Status = await serverApi.Restart( cancellation ),
        };
    }

    internal static async IAsyncEnumerable<ProcessState> StopServer( IServerApi serverApi, ProcessState state, [EnumeratorCancellation] CancellationToken cancellation )
    {
        ArgumentNullException.ThrowIfNull( serverApi );
        ArgumentNullException.ThrowIfNull( state );

        yield return state with
        {
            IsServerResetting = true
        };

        yield return state with
        {
            IsServerResetting = false,
            Status = await serverApi.Terminate( cancellation ),
        };
    }
}