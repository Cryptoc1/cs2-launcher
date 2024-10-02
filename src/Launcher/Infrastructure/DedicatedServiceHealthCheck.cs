using CS2Launcher.AspNetCore.App.Abstractions.Api;
using CS2Launcher.AspNetCore.Launcher.Abstractions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CS2Launcher.AspNetCore.Launcher.Infrastructure;

internal sealed class DedicatedServerHealthCheck( IDedicatedServer server ) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync( HealthCheckContext context, CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull( context );

        if( !server.Options.Enabled )
        {
            return Task.FromResult( HealthCheckResult.Healthy( $"{nameof( IDedicatedServer )} is disabled." ) );
        }

        return Task.FromResult( server.Status switch
        {
            ServerStatus.Starting or ServerStatus.Running => HealthCheckResult.Healthy( $"{nameof( IDedicatedServer )} is {server.Status}." ),
            _ => HealthCheckResult.Degraded( $"{nameof( IDedicatedServer )} is {server.Status}." ),
        } );
    }
}