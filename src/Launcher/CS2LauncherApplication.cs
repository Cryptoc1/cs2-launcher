using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CS2Launcher.AspNetCore.Launcher;

/// <summary> The launcher application used to configure CS2 and the underlying <see cref="WebApplication"/> host. </summary>
public sealed class CS2LauncherApplication : IAsyncDisposable
{
    /// <summary> The underlying host of the launcher. </summary>
    public WebApplication WebApp { get; }

    internal CS2LauncherApplication( WebApplication app ) => WebApp = app;

    /// <summary> Initialize a <see cref="CS2LauncherApplicationBuilder"/> with pre-configured defaults. </summary>
    /// <param name="args"> The command line arguments.  </param>
    public static CS2LauncherApplicationBuilder CreateBuilder( string[] args )
    {
        var builder = WebApplication.CreateBuilder( args );
        builder.Configuration.AddEnvironmentVariables( prefix: "CS2L_" );
        builder.Services.AddCS2Launcher();

        return new( builder );
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync( ) => WebApp.DisposeAsync();

    /// <summary> Runs the applications. </summary>
    /// <returns> A task that completes when the application is shutdown. </returns>
    /// <remarks> If the <see cref="Proc.DedicatedServerProcess"/> crashes, the host application will be shutdown. </remarks>
    public Task RunAsync( ) => WebApp.RunAsync();
}

/// <summary> A builder for a CS2 Launcher and its services. </summary>
public sealed class CS2LauncherApplicationBuilder : IHostApplicationBuilder
{
    private readonly WebApplicationBuilder builder;

    internal CS2LauncherApplicationBuilder( WebApplicationBuilder builder ) => this.builder = builder;

    /// <inheritdoc/>
    public IConfigurationManager Configuration => builder.Configuration;

    /// <inheritdoc/>
    public IHostEnvironment Environment => builder.Environment;

    /// <inheritdoc/>
    public ILoggingBuilder Logging => builder.Logging;

    /// <inheritdoc/>
    public IMetricsBuilder Metrics => builder.Metrics;

    /// <inheritdoc/>
    public IDictionary<object, object> Properties => (( IHostApplicationBuilder )builder).Properties;

    /// <inheritdoc/>
    public IServiceCollection Services => builder.Services;

    /// <summary> Build the CS2 launcher application. </summary>
    public CS2LauncherApplication Build( ) => new( builder.Build() );

    /// <inheritdoc/>
    public void ConfigureContainer<TContainerBuilder>( IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder>? configure = null )
        where TContainerBuilder : notnull
        => (( IHostApplicationBuilder )builder).ConfigureContainer( factory, configure );
}