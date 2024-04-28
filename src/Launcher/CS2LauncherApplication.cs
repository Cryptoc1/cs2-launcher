using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using CS2Launcher.AspNetCore.App;
using CS2Launcher.AspNetCore.App.Hosting;
using CS2Launcher.AspNetCore.Launcher.Abstractions;
using CS2Launcher.AspNetCore.Launcher.Authorization;
using CS2Launcher.AspNetCore.Launcher.Configuration;
using CS2Launcher.AspNetCore.Launcher.Hosting;
using CS2Launcher.AspNetCore.Launcher.Proc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
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
    internal readonly WebApplication WebApp;

    IServiceProvider IEndpointRouteBuilder.ServiceProvider => (( IEndpointRouteBuilder )WebApp).ServiceProvider;
    ICollection<EndpointDataSource> IEndpointRouteBuilder.DataSources => (( IEndpointRouteBuilder )WebApp).DataSources;

    IServiceProvider IHost.Services => WebApp.Services;

    IServiceProvider IApplicationBuilder.ApplicationServices { get => (( IApplicationBuilder )WebApp).ApplicationServices; set => (( IApplicationBuilder )WebApp).ApplicationServices = value; }
    IFeatureCollection IApplicationBuilder.ServerFeatures => (( IApplicationBuilder )WebApp).ServerFeatures;
    IDictionary<string, object?> IApplicationBuilder.Properties => (( IApplicationBuilder )WebApp).Properties;

    internal CS2LauncherApplication( WebApplication app ) => WebApp = app;

    RequestDelegate IApplicationBuilder.Build( ) => (( IApplicationBuilder )WebApp).Build();

    IApplicationBuilder IEndpointRouteBuilder.CreateApplicationBuilder( ) => (( IEndpointRouteBuilder )WebApp).CreateApplicationBuilder();

    /// <summary> Initialize a <see cref="CS2LauncherApplicationBuilder"/> with pre-configured defaults. </summary>
    /// <param name="args"> The command line arguments.  </param>
    public static CS2LauncherApplicationBuilder CreateBuilder( string[] args )
    {
        var builder = WebApplication.CreateBuilder( args );
        builder.Configuration.AddEnvironmentVariables( prefix: "CS2L_" );

        builder.Services.AddCors()
            .AddRequestDecompression()
            .AddResponseCaching()
            .AddResponseCompression()
            .AddRouting( options => options.LowercaseUrls = true );

        builder.Services.AddHostedService<DedicatedServer>()
            .AddOptions<DedicatedServerOptions>()
            .BindConfiguration( "Server" )
            .ValidateDataAnnotations();

        return new( builder );
    }

    void IDisposable.Dispose( ) => (WebApp as IDisposable)?.Dispose();

    /// <inheritdoc/>
    public ValueTask DisposeAsync( ) => WebApp.DisposeAsync();

    IApplicationBuilder IApplicationBuilder.New( ) => (( IApplicationBuilder )WebApp).New();

    /// <summary> Runs the applications. </summary>
    /// <returns> A task that completes when the application is shutdown. </returns>
    /// <remarks> If the <see cref="DedicatedServerProcess"/> crashes, the host application will be shutdown. </remarks>
    public Task RunAsync( ) => WebApp.RunAsync();

    Task IHost.StartAsync( CancellationToken cancellation ) => WebApp.StartAsync( cancellation );

    Task IHost.StopAsync( CancellationToken cancellation ) => WebApp.StopAsync( cancellation );

    IApplicationBuilder IApplicationBuilder.Use( Func<RequestDelegate, RequestDelegate> middleware ) => WebApp.Use( middleware );
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
        if( app.Environment.IsDevelopment() )
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseForwardedHeaders();
        app.UseHttpsRedirection();
        app.UseCookiePolicy();
        app.UseCors();

        app.UseRequestDecompression();
        app.UseResponseCaching();
        if( !app.Environment.IsDevelopment() )
        {
            app.UseResponseCompression();
        }

        if( app.Services.GetService<RootComponentDescriptor>() is not null )
        {
            app.UseLauncherApp();
        }
        else
        {
            app.Map( "/", ( ) => Results.NoContent() );
        }

        return new( app );
    }

    /// <inheritdoc/>
    public void ConfigureContainer<TContainerBuilder>( IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder>? configure = null )
        where TContainerBuilder : notnull
        => (( IHostApplicationBuilder )builder).ConfigureContainer( factory, configure );

    /// <summary> Host the Launcher App with the root component of type <typeparamref name="TRoot"/>. </summary>
    /// <typeparam name="TRoot"> The root component of the Launcher App. </typeparam>
    public CS2LauncherApplicationBuilder WithLauncherApp<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.All )] TRoot>( )
        where TRoot : RootComponent
    {
        Services.AddCS2LauncherApp<TRoot>()
            .AddSingleton( LauncherHostContext.From( Assembly.GetEntryAssembly()! ) )
            .AddControllersWithViews();

        Services.AddAuthorization()
            .AddSingleton<IAuthorizationHandler, AppUserAuthorizationHandler>()
            .AddAuthentication( CookieAuthenticationDefaults.AuthenticationScheme )
            .AddCookie()
            .AddSteam();

        Services.AddSignalR()
#if DEBUG
            .AddJsonProtocol()
#endif
            .AddMessagePackProtocol();

        Services.AddOptions<LauncherAppOptions>()
            .BindConfiguration( "App" )
            .ValidateDataAnnotations();

        Services.ConfigureOptions<ConfigureAuthentication>()
            .ConfigureOptions<ConfigureAuthorization>()
            .ConfigureOptions<ConfigureResponseCompression>();

        return this;
    }
}