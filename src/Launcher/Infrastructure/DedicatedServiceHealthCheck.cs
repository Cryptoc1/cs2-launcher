using CS2Launcher.AspNetCore.App.Abstractions.Api;
using CS2Launcher.AspNetCore.Launcher.Abstractions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CS2Launcher.AspNetCore.Launcher.Infrastructure;

internal sealed class DedicatedServerHealthCheck( IDedicatedServer server ) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync( HealthCheckContext context, CancellationToken cancellation = default )
    {
        ArgumentNullException.ThrowIfNull( context );

        return await server.Status( cancellation ) switch
        {
            ServerStatus.Starting or ServerStatus.Running => HealthCheckResult.Healthy( $"{nameof( IDedicatedServer )} is {server.Status}." ),
            _ => HealthCheckResult.Degraded( $"{nameof( IDedicatedServer )} is {server.Status}." ),
        };
    }
}