using System.Net.Http.Json;
using CS2Launcher.AspNetCore.App.Abstractions.Api;
using CS2Launcher.AspNetCore.App.Json;

namespace CS2Launcher.AspNetCore.App.Infrastructure;

internal sealed class LauncherApiClient( HttpClient http ) : ILauncherApiClient
{
    public IServerApi Server { get; } = new ServerApi( http );
}

sealed file class ServerApi( HttpClient http ) : IServerApi
{
    public async Task<bool> ChangeMap( ChangeMapParameters parameters, CancellationToken cancellation = default )
    {
        ArgumentNullException.ThrowIfNull( parameters );

        using var response = await http.PostAsJsonAsync( "server/map", parameters, AppJsonContext.Default.ChangeMapParameters, cancellation );
        return await response.Content.ReadFromJsonAsync( AppJsonContext.Default.Boolean, cancellation );
    }

    public async Task<ServerMetrics> Metrics( CancellationToken cancellation = default )
        => await http.GetFromJsonAsync( "server/metrics", AppJsonContext.Default.ServerMetrics, cancellation ) ?? ServerMetrics.Zero;

    public async Task<ServerStatus> Restart( CancellationToken cancellation = default )
    {
        using var response = await http.PutAsync( "server/restart", default, cancellation );
        return await response.Content.ReadFromJsonAsync( AppJsonContext.Default.ServerStatus, cancellation );
    }

    public Task<ServerStatus> Status( CancellationToken cancellation = default )
        => http.GetFromJsonAsync( "server/status", AppJsonContext.Default.ServerStatus, cancellation );

    public async Task<ServerStatus> Terminate( CancellationToken cancellation = default )
    {
        using var response = await http.PutAsync( "server/terminate", default, cancellation );
        return await response.Content.ReadFromJsonAsync( AppJsonContext.Default.ServerStatus, cancellation );
    }
}

