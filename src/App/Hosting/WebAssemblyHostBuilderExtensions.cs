using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;

namespace CS2Launcher.AspNetCore.App.Hosting;

/// <summary> Extensions for configuring a CS2Launcher App in a Web Assembly Host. </summary>
public static class WebAssemblyHostBuilderExtensions
{
    /// <summary> Configure the Web Assembly Host to use the CS2Launcher App with the root component of type <typeparamref name="TRoot"/>. </summary>
    /// <typeparam name="TRoot"> The type of <see cref="RootComponent"/>. </typeparam>
    public static WebAssemblyHostBuilder UseCS2Launcher<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.All )] TRoot>( this WebAssemblyHostBuilder builder )
        where TRoot : RootComponent
    {
        ArgumentNullException.ThrowIfNull( builder );

        builder.Logging.SetMinimumLevel( builder.HostEnvironment.IsDevelopment()
            ? LogLevel.Information
            : LogLevel.Error );

        builder.Services.AddCS2LauncherApp<TRoot>();
        return builder;
    }
}