using System.Diagnostics.CodeAnalysis;
using CS2Launcher.AspNetCore.App.Interop;
using CS2Launcher.AspNetCore.App.Shared;

namespace CS2Launcher.AspNetCore.App;

public static class AppServiceExtensions
{
    [DynamicDependency( DynamicallyAccessedMemberTypes.All, typeof( AppRoot ) )]
    [DynamicDependency( DynamicallyAccessedMemberTypes.All, typeof( Console ) )]
    [DynamicDependency( DynamicallyAccessedMemberTypes.All, typeof( Layout ) )]
    public static IServiceCollection AddCS2LauncherApp( this IServiceCollection services )
    {
        ArgumentNullException.ThrowIfNull( services );

        // EnsureRuntimeRefs();
        return services.AddScoped<ElementInterop>();
    }

    // [DynamicDependency( DynamicallyAccessedMemberTypes.All, typeof( AppRoot ) )]
    // internal static void EnsureRuntimeRefs( ) => typeof( AppRoot ).GetTypeInfo();
}