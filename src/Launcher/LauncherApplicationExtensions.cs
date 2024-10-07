using CS2Launcher.AspNetCore.Launcher.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace CS2Launcher.AspNetCore.Launcher;

internal static class LauncherApplicationExtensions
{
    internal static WebApplication UseLauncherApp( this WebApplication app )
    {
        ArgumentNullException.ThrowIfNull( app );
        if( app.Environment.IsDevelopment() )
        {
            app.UseWebAssemblyDebugging();
        }

        app.UseAuthentication();
        app.UseAuthorization();

#if NET8_0
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles( new StaticFileOptions { ServeUnknownFileTypes = true } );
#endif

#if NET9_0_OR_GREATER
        app.MapStaticAssets()
            .WithRequestTimeout( TimeSpan.FromMinutes( 2 ) );
#endif

        app.MapHub<ConsoleHub>( "/api/signals/console" )
            .RequireAuthorization()
            .WithRequestTimeout( TimeSpan.FromMinutes( 2 ) );

        app.MapHub<MetricsHub>( "/api/signals/metrics" )
            .RequireAuthorization()
            .WithRequestTimeout( TimeSpan.FromMinutes( 2 ) );

        app.MapControllers()
            .RequireAuthorization()
            .WithRequestTimeout( TimeSpan.FromMinutes( 2 ) );

        app.MapFallbackToController( "Index", "App" )
            .RequireAuthorization()
            .WithRequestTimeout( TimeSpan.FromMinutes( 2 ) );

        return app;
    }
}