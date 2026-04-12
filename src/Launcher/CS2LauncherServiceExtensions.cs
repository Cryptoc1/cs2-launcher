using CS2Launcher.AspNetCore.Launcher.Abstractions;
using CS2Launcher.AspNetCore.Launcher.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace CS2Launcher.AspNetCore.Launcher;

internal static class CS2LauncherServiceExtensions
{
    public static IServiceCollection AddDedicatedServer( this IServiceCollection services )
    {
        ArgumentNullException.ThrowIfNull( services );

        services.AddSingleton<DedicatedServer>()
            .AddSingleton<IDedicatedServer>( serviceProvider => serviceProvider.GetRequiredService<DedicatedServer>() )
            .AddHostedService( serviceProvider => serviceProvider.GetRequiredService<DedicatedServer>() );

#pragma warning disable IL2026,IL3050
        services.AddOptions<DedicatedServerOptions>()
#pragma warning restore IL2026,IL3050

            .BindConfiguration( "Server" )
            .ValidateDataAnnotations()
            .Validate( options => !options.Enabled || !string.IsNullOrWhiteSpace( options.Program ), $"The {nameof( DedicatedServerOptions )}.{nameof( DedicatedServerOptions.Enabled )} == True, but a {nameof( DedicatedServerOptions.Program )} was not specified." );

        services.AddHealthChecks()
            .AddCheck<DedicatedServerHealthCheck>( nameof( DedicatedServer ) );

        return services;
    }

    public static IServiceCollection AddServerInstaller( this IServiceCollection services )
    {
        ArgumentNullException.ThrowIfNull( services );

        services.AddSingleton<ServerInstaller>()
            .AddSingleton<IServerInstaller>( serviceProvider => serviceProvider.GetRequiredService<ServerInstaller>() )
            .AddHostedService( serviceProvider => serviceProvider.GetRequiredService<ServerInstaller>() );

#pragma warning disable IL2026,IL3050
        services.AddOptions<ServerInstallerOptions>()
#pragma warning restore IL2026,IL3050

            .BindConfiguration( "Installer" )
            .ValidateDataAnnotations();

        return services;
    }
}