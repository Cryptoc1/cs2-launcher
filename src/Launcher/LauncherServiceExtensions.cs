using CS2Launcher.AspNetCore.Launcher.Proc;
using Microsoft.Extensions.DependencyInjection;

namespace CS2Launcher.AspNetCore.Launcher;

/// <summary> Extensions for register services required by launchers. </summary>
public static class LauncherServiceExtensions
{
    /// <summary> Adds services for CS2 launcher applications. </summary>
    /// <see cref="DedicatedServer" />
    public static IServiceCollection AddCS2Launcher( this IServiceCollection services )
    {
        ArgumentNullException.ThrowIfNull( services );

#pragma warning disable IL2026,IL3050
        services.AddHostedService<DedicatedServer>()
            .AddOptions<DedicatedServerOptions>()
            .BindConfiguration( "Server" )
            .ValidateDataAnnotations();
#pragma warning restore IL2026,IL3050

        return services;
    }
}