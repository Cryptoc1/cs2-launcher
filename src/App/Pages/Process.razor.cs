using CS2Launcher.AspNetCore.App.Abstractions.Api;
using CS2Launcher.AspNetCore.App.Abstractions.Signals;
using CS2Launcher.AspNetCore.App.Components;
using CS2Launcher.AspNetCore.App.Infrastructure;

namespace CS2Launcher.AspNetCore.App.Pages;

public sealed record ProcessState : State
{
    public bool IsLoading { get; init; } = true;
    public bool IsServerResetting { get; init; }
    public ServerMetrics Metrics { get; init; } = ServerMetrics.Zero;
    public ServerStatus Status { get; init; }

    internal static async IAsyncEnumerable<ProcessState> Load( IServerApi serverApi, MetricsSignaler signaler, ProcessState state )
    {
        ArgumentNullException.ThrowIfNull( serverApi );
        ArgumentNullException.ThrowIfNull( state );

        var connect = signaler.Connect();
        yield return await Load( serverApi, state );

        await connect;

        static async Task<ProcessState> Load( IServerApi serverApi, ProcessState state )
        {
            var metrics = serverApi.Metrics();
            var status = serverApi.Status();

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

    internal static async IAsyncEnumerable<ProcessState> RestartServer( IServerApi serverApi, ProcessState state )
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
            Status = await serverApi.Restart(),
        };
    }

    internal static async IAsyncEnumerable<ProcessState> StopServer( IServerApi serverApi, ProcessState state )
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
            Status = await serverApi.Terminate(),
        };
    }
}