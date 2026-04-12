using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace CS2Launcher.Aspire.Hosting;

/// <summary> Extensions for managing CS2Launcher resources. </summary>
public static class CS2LauncherResourceBuilder
{
    private const string LaunchProfileName = "cs2-launcher";

    /// <summary> Add a CS2Launcher project to the application model. </summary>
    public static IResourceBuilder<ProjectResource> AddCS2Launcher<TProject>(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name )
        where TProject : IProjectMetadata, new()
    {
        ArgumentNullException.ThrowIfNull( builder );

        var resource = builder.AddProject<TProject>( name, LaunchProfileName )
            .WithIconName( "game" );

        return resource;
    }
}