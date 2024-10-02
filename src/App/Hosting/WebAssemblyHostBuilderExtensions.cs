using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using CS2Launcher.AspNetCore.App.Infrastructure;
using ESCd.Extensions.Http;
using CS2Launcher.AspNetCore.App.Abstractions.Api;

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

        builder.Services.AddTransient<ApiProblemHandler>()
            .AddQueryStringBuilderObjectPool()
            .AddHttpClient<ILauncherApiClient, LauncherApiClient>( http => http.BaseAddress = new( builder.HostEnvironment.BaseAddress + "api/" ) )
            .AddHttpMessageHandler<ApiProblemHandler>();

        builder.Services.AddCS2LauncherApp<TRoot>();
        return builder;
    }
}