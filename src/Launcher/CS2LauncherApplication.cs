using CS2Launcher.AspNetCore.Launcher.Ingest;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CS2Launcher.AspNetCore.Launcher;

public sealed class CS2LauncherApplication : IAsyncDisposable
{
    public WebApplication WebApp { get; }

    internal CS2LauncherApplication( WebApplication app ) => WebApp = app;

    public static CS2LauncherApplicationBuilder CreateBuilder( string[] args )
    {
        var builder = WebApplication.CreateBuilder( args );
        builder.Configuration.AddEnvironmentVariables( prefix: "CS2L_" );

        builder.Services.AddCS2Launcher();
        return new( builder );
    }

    public ValueTask DisposeAsync( ) => WebApp.DisposeAsync();

    public Task RunAsync( ) => WebApp.RunAsync();
}

public sealed class CS2LauncherApplicationBuilder : IHostApplicationBuilder
{
    private readonly WebApplicationBuilder builder;

    internal CS2LauncherApplicationBuilder( WebApplicationBuilder builder ) => this.builder = builder;

    public IConfigurationManager Configuration => builder.Configuration;
    public IHostEnvironment Environment => builder.Environment;
    public ILoggingBuilder Logging => builder.Logging;
    public IMetricsBuilder Metrics => builder.Metrics;
    public IDictionary<object, object> Properties => (( IHostApplicationBuilder )builder).Properties;
    public IServiceCollection Services => builder.Services;

    public CS2LauncherApplication Build( )
    {
        var app = builder.Build();
        app.MapPost( "/ingest", IngestEndpoint.Ingest );

        return new( app );
    }

    public void ConfigureContainer<TContainerBuilder>( IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder>? configure = null )
        where TContainerBuilder : notnull
        => (( IHostApplicationBuilder )builder).ConfigureContainer( factory, configure );
}