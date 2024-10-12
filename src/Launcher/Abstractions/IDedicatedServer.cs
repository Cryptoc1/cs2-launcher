using CS2Launcher.AspNetCore.App.Abstractions.Api;

namespace CS2Launcher.AspNetCore.Launcher.Abstractions;

/// <summary> Describes a service for interacting with the underlying dedicated server process. </summary>
public interface IDedicatedServer
{
    /// <summary> Retrieve performance metrics of the server. </summary>
    ValueTask<ServerMetrics> Metrics( CancellationToken cancellation );

    /// <summary> Restart the underlying process of the server. </summary>
    ValueTask Restart( CancellationToken cancellation );

    /// <summary> The current status of the server. </summary>
    ValueTask<ServerStatus> Status( CancellationToken cancellation );

    /// <summary> Terminate the underlying process of the server. </summary>
    ValueTask Terminate( CancellationToken cancellation );
}