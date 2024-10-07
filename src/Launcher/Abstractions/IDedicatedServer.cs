using CS2Launcher.AspNetCore.App.Abstractions.Api;
using Microsoft.Extensions.Hosting;

namespace CS2Launcher.AspNetCore.Launcher.Abstractions;

/// <summary> Describes the "Dedicated Server" Hosted Service. </summary>
public interface IDedicatedServer : IHostedService
{
    /// <summary> Retrieve performance metrics for the server. </summary>
    ValueTask<ServerMetrics> Metrics( );

    /// <summary> The current status of the server. </summary>
    ValueTask<ServerStatus> Status( );
}