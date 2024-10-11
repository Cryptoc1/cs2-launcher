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

        if( await server.Status( cancellation ) is not ServerStatus.Running )
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

    public async Task<ServerMetrics> Metrics( CancellationToken cancellation = default ) => await server.Metrics( cancellation );

    public async Task<ServerStatus> Restart( CancellationToken cancellation )
    {
        server.Restart();
        return await server.Status( cancellation );
    }

    public async Task<ServerStatus> Status( CancellationToken cancellation = default ) => await server.Status( cancellation );

    public async Task<ServerStatus> Terminate( CancellationToken cancellation = default )
    {
        server.Terminate();
        return await server.Status( cancellation );
    }
}