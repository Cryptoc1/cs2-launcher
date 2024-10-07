using CS2Launcher.AspNetCore.App.Abstractions.Api;
using CS2Launcher.AspNetCore.App.Abstractions.Signals;
using CS2Launcher.AspNetCore.App.Components;
using CS2Launcher.AspNetCore.App.Infrastructure;

namespace CS2Launcher.AspNetCore.App.Pages;

public sealed record ProcessState : State
{
    public bool IsLoading { get; init; } = true;
    public ServerMetrics Metrics { get; init; } = ServerMetrics.Zero;

    internal static async IAsyncEnumerable<ProcessState> Load( IServerApi serverApi, MetricsSignaler signaler, ProcessState state )
    {
        ArgumentNullException.ThrowIfNull( serverApi );
        ArgumentNullException.ThrowIfNull( state );

        var connect = signaler.Connect();
        yield return await Load( serverApi, state );

        await connect;

        static async Task<ProcessState> Load( IServerApi serverApi, ProcessState state )
            => state with
            {
                IsLoading = false,
                Metrics = await serverApi.Metrics()
            };
    }

    internal static ProcessState OnReport( MetricsSignals.Report report, ProcessState state )
        => state with
        {
            Metrics = report.Metrics
        };
}