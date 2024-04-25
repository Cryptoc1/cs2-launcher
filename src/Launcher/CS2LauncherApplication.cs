using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CS2Launcher.AspNetCore.Launcher;

/// <summary> The launcher application used to configure CS2 and the underlying <see cref="WebApplication"/> host. </summary>
public sealed class CS2LauncherApplication : IApplicationBuilder, IAsyncDisposable, IEndpointRouteBuilder, IHost
{
    /// <summary> The underlying host of the launcher. </summary>
    internal readonly WebApplication app;

    IServiceProvider IEndpointRouteBuilder.ServiceProvider => (( IEndpointRouteBuilder )app).ServiceProvider;
    ICollection<EndpointDataSource> IEndpointRouteBuilder.DataSources => (( IEndpointRouteBuilder )app).DataSources;

    IServiceProvider IHost.Services => app.Services;

    IServiceProvider IApplicationBuilder.ApplicationServices { get => (( IApplicationBuilder )app).ApplicationServices; set => (( IApplicationBuilder )app).ApplicationServices = value; }
    IFeatureCollection IApplicationBuilder.ServerFeatures => (( IApplicationBuilder )app).ServerFeatures;
    IDictionary<string, object?> IApplicationBuilder.Properties => (( IApplicationBuilder )app).Properties;

    internal CS2LauncherApplication( WebApplication app ) => this.app = app;

    RequestDelegate IApplicationBuilder.Build( ) => (( IApplicationBuilder )app).Build();

    IApplicationBuilder IEndpointRouteBuilder.CreateApplicationBuilder( ) => (( IEndpointRouteBuilder )app).CreateApplicationBuilder();

    /// <summary> Initialize a <see cref="CS2LauncherApplicationBuilder"/> with pre-configured defaults. </summary>
    /// <param name="args"> The command line arguments.  </param>
    public static CS2LauncherApplicationBuilder CreateBuilder( string[] args )
    {
        var builder = WebApplication.CreateBuilder( args );
        builder.Configuration.AddEnvironmentVariables( prefix: "CS2L_" );
        builder.Services.AddCS2Launcher();

        return new( builder );
    }

    void IDisposable.Dispose( ) => (app as IDisposable)?.Dispose();

    /// <inheritdoc/>
    public ValueTask DisposeAsync( ) => app.DisposeAsync();

    IApplicationBuilder IApplicationBuilder.New( ) => (( IApplicationBuilder )app).New();

    /// <summary> Runs the applications. </summary>
    /// <returns> A task that completes when the application is shutdown. </returns>
    /// <remarks> If the <see cref="Proc.DedicatedServerProcess"/> crashes, the host application will be shutdown. </remarks>
    public Task RunAsync( ) => app.RunAsync();

    Task IHost.StartAsync( CancellationToken cancellation ) => app.StartAsync( cancellation );

    Task IHost.StopAsync( CancellationToken cancellation ) => app.StopAsync( cancellation );

    IApplicationBuilder IApplicationBuilder.Use( Func<RequestDelegate, RequestDelegate> middleware ) => app.Use( middleware );
}

/// <summary> A builder for a CS2 Launcher and its services. </summary>
public sealed class CS2LauncherApplicationBuilder : IHostApplicationBuilder
{
    private readonly WebApplicationBuilder builder;

    internal CS2LauncherApplicationBuilder( WebApplicationBuilder builder ) => this.builder = builder;

    /// <inheritdoc/>
    public IConfigurationManager Configuration => builder.Configuration;

    /// <summary> An <see cref="IHostBuilder"/> for configuring host specific properties. </summary>
    public ConfigureHostBuilder Host => builder.Host;

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

    /// <summary> An <see cref="IWebHostBuilder"/> for configuring server specific properties. </summary>
    public ConfigureWebHostBuilder WebHost => builder.WebHost;

    /// <summary> Build the CS2 launcher application. </summary>
    public CS2LauncherApplication Build( )
    {
        var app = builder.Build();
        app.UseCS2Launcher();

        return new( app );
    }

    /// <inheritdoc/>
    public void ConfigureContainer<TContainerBuilder>( IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder>? configure = null )
        where TContainerBuilder : notnull
        => (( IHostApplicationBuilder )builder).ConfigureContainer( factory, configure );
}