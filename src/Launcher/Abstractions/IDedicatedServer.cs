using CS2Launcher.AspNetCore.App.Abstractions.Api;
using Microsoft.Extensions.Hosting;

namespace CS2Launcher.AspNetCore.Launcher.Abstractions;

/// <summary> Describes the "Dedicated Server" Hosted Service. </summary>
public interface IDedicatedServer : IHostedService
{
    /// <summary> The <see cref="DedicatedServerOptions"/> used to configure the server. </summary>
    DedicatedServerOptions Options { get; }

    /// <summary> The current status of the server. </summary>
    ServerStatus Status { get; }

    /// <summary> Retrieve performance metrics for the server. </summary>
    ServerMetrics GetMetrics( );
}