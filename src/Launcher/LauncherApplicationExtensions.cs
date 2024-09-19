using CS2Launcher.AspNetCore.Launcher.Api;
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

#if NET8_0
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles( new StaticFileOptions { ServeUnknownFileTypes = true } );
#endif

        app.UseAuthentication();
        app.UseAuthorization();

#if NET9_0_OR_GREATER
        app.MapStaticAssets()
            .RequireAuthorization()
            .WithRequestTimeout( TimeSpan.FromMinutes( 2 ) );
#endif

        app.MapHub<ConsoleHub>( "/api/signals/console", options => options.AllowStatefulReconnects = true );

        app.MapFallbackToController( "Index", "App" )
            .WithRequestTimeout( TimeSpan.FromMinutes( 2 ) );

        return app;
    }
}