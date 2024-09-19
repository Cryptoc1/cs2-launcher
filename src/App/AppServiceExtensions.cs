using System.Diagnostics.CodeAnalysis;
using CS2Launcher.AspNetCore.App.Hosting;
using CS2Launcher.AspNetCore.App.Interop;
using Microsoft.Extensions.DependencyInjection;

namespace CS2Launcher.AspNetCore.App;

/// <summary> Extensions for registering services required by the CS2Launcher App. </summary>
public static class AppServiceExtensions
{
    /// <summary> Add services required by the CS2Launcher App. </summary>
    /// <typeparam name="TRoot"> The type of <see cref="RootComponent"/> of the app. </typeparam>
    [DynamicDependency( DynamicallyAccessedMemberTypes.All, typeof( Shared.Layout ) )]
    public static IServiceCollection AddCS2LauncherApp<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.All )] TRoot>( this IServiceCollection services )
        where TRoot : RootComponent
    {
        ArgumentNullException.ThrowIfNull( services );
        return services.AddSingleton( new RootComponentDescriptor( typeof( TRoot ) ) )
            .AddScoped<ElementInterop>()
            .AddScoped<LocalStorageInterop>();
    }
}