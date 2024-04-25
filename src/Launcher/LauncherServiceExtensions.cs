using System.Reflection;
using AspNet.Security.OpenId.Steam;
using CS2Launcher.AspNetCore.App.Abstractions;
using CS2Launcher.AspNetCore.Launcher.Configuration;
using CS2Launcher.AspNetCore.Launcher.Hosting;
using CS2Launcher.AspNetCore.Launcher.Proc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
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

        services.AddHostedService<DedicatedServer>()
            .AddOptions<DedicatedServerOptions>()
            .BindConfiguration( "Server" )
            .ValidateDataAnnotations();

        services.AddControllersWithViews();
        services.AddRazorComponents()
            .AddInteractiveWebAssemblyComponents();

        services.AddAuthorization()
            .AddAuthentication( CookieAuthenticationDefaults.AuthenticationScheme )
            .AddCookie()
            .AddSteam();

        services.AddSignalR()
            .AddJsonProtocol()
            .AddMessagePackProtocol();

        services.AddRequestDecompression()
            .AddResponseCompression();

        services.AddSingleton( new LauncherHostContext( Assembly.GetEntryAssembly()! ) )
            .AddTransient<HostContext>( serviceProvider => serviceProvider.GetRequiredService<LauncherHostContext>() );

        return services.ConfigureOptions<ConfigureAuthentication>()
            .ConfigureOptions<ConfigureResponseCompression>();
    }
}