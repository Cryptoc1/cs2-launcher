using CoreRCON;
using CoreRCON.Extensions.CounterStrike;
using CS2Launcher.AspNetCore.App.Abstractions.Api;
using CS2Launcher.AspNetCore.App.Annotations;
using CS2Launcher.AspNetCore.Launcher.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CS2Launcher.AspNetCore.Launcher.Infrastructure;

internal sealed class LauncherApiClient( IServiceProvider serviceProvider ) : ILauncherApiClient
{
    public IServerApi Server { get; } = ActivatorUtilities.CreateInstance<ServerApi>( serviceProvider );
}

sealed file class ServerApi(
    IServerConsoleFactory consoleFactory,
    IDedicatedServer server ) : IServerApi
{
    public async Task<bool> ChangeMap( ChangeMapParameters parameters, CancellationToken cancellation = default )
    {
        ArgumentNullException.ThrowIfNull( parameters );

        if( server.Status is not ServerStatus.Running )
        {
            return false;
        }

        if( !WorkshopIdParser.TryParseId( parameters.WorkshopId, out var workshopId ) )
        {
            return false;
        }

        try
        {
            using var console = consoleFactory.Create();
            return await console.DSWorkshopChangeLevel( workshopId.ToString(), cancellation ) is "";
        }
        catch( RCONException )
        {
            return false;
        }
    }

    public Task<ServerMetrics> Metrics( CancellationToken cancellation = default ) => Task.FromResult( server.GetMetrics() );

    public Task<ServerStatus> Status( CancellationToken cancellation = default ) => Task.FromResult( server.Status );
}