using Microsoft.AspNetCore.Components.Endpoints;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace CS2Launcher.AspNetCore.Launcher.Configuration;

internal sealed class ConfigureMvc( IWebHostEnvironment environment ) : IConfigureOptions<RazorComponentsServiceOptions>, IConfigureOptions<RouteOptions>
{
    public void Configure( RazorComponentsServiceOptions options )
    {
        ArgumentNullException.ThrowIfNull( options );
        options.DetailedErrors = environment.IsDevelopment();
    }

    public void Configure( RouteOptions options )
    {
        ArgumentNullException.ThrowIfNull( options );
        options.AppendTrailingSlash = false;
        options.LowercaseUrls = true;
    }
}