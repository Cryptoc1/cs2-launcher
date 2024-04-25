using CS2Launcher.AspNetCore.Launcher.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace CS2Launcher.AspNetCore.Launcher;

internal static class WebApplicationExtensions
{
    public static WebApplication UseCS2Launcher( this WebApplication app )
    {
        ArgumentNullException.ThrowIfNull( app );

        // Configure the HTTP request pipeline.
        if( app.Environment.IsDevelopment() )
        {
            app.UseWebAssemblyDebugging();
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler( "/error", createScopeForErrors: true );
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseCookiePolicy();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles( new StaticFileOptions { ServeUnknownFileTypes = true } );
        app.UseCors( policy => policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials() );

        app.UseRequestDecompression();
        app.UseResponseCompression();
        app.UseResponseCaching();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHub<ConsoleHub>( "/api/signals/console", options => options.AllowStatefulReconnects = true );
        app.MapFallbackToController( "Index", "App" );

        return app;
    }
}