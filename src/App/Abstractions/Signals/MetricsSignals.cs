using CS2Launcher.AspNetCore.App.Abstractions.Api;

namespace CS2Launcher.AspNetCore.App.Abstractions.Signals;

/// <summary> Signals used by the 'Metrics' feature. </summary>
public static class MetricsSignals
{
    /// <summary> Represents the 'Report' signal. </summary>
    /// <param name="Metrics"> The server's metrics being reported. </param>
    public sealed record Report( ServerMetrics Metrics ) : Signal<Report>;
}