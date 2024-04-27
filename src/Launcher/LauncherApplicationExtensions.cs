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

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles( new StaticFileOptions { ServeUnknownFileTypes = true } );

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHub<ConsoleHub>( "/api/signals/console", options => options.AllowStatefulReconnects = true );
        app.MapFallbackToController( "Index", "App" );

        return app;
    }
}