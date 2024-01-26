using CS2Launcher.AspNetCore.Launcher.Proc;
using Microsoft.Extensions.DependencyInjection;

namespace CS2Launcher.AspNetCore.Launcher;

public static class LauncherServiceExtensions
{
    public static IServiceCollection AddCS2Launcher( this IServiceCollection services )
    {
        ArgumentNullException.ThrowIfNull( services );

        services.AddHostedService<DedicatedServer>()
            .AddOptions<DedicatedServerOptions>()
            .BindConfiguration( "Server" )
            .ValidateDataAnnotations();

        return services;
    }
}