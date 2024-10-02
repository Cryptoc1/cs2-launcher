using System.Diagnostics.CodeAnalysis;
using CS2Launcher.AspNetCore.App.Abstractions.Signals;
using CS2Launcher.AspNetCore.App.Infrastructure;
using CS2Launcher.AspNetCore.App.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal abstract class Signaler( Action<IHubConnectionBuilder> configure ) : IAsyncDisposable
{
    private readonly Lazy<HubConnection> connection = new( ConnectionFactory( configure ) );
    protected HubConnection Connection => connection.Value;

    protected Signaler( NavigationManager navigation, string path )
        : this( connection => connection.WithUrl( navigation.ToAbsoluteUri( path ) ) )
    {
    }

    private static Func<HubConnection> ConnectionFactory( Action<IHubConnectionBuilder> configure )
        => ( ) =>
        {
            var builder = new HubConnectionBuilder()
                .AddJsonProtocol( options => options.PayloadSerializerOptions.TypeInfoResolverChain.Add( AppJsonContext.Default ) )
#if DEBUG
                .ConfigureLogging( logging => logging.SetMinimumLevel( LogLevel.Debug ) )
#endif
                .WithAutomaticReconnect()
                .WithStatefulReconnect();

            configure( builder );
            return builder.Build();
        };

    public async ValueTask DisposeAsync( )
    {
        if( connection.IsValueCreated )
        {
            await connection.Value.DisposeAsync();
        }

        GC.SuppressFinalize( this );
    }

    public Task Connect( )
    {
        PlatformGuard.ThrowIfNotBrowser( "Signaler cannot be initialized outside of a browser context." );
        if( Connection.State > HubConnectionState.Disconnected )
        {
            return Task.CompletedTask;
        }

        return Connection.StartAsync();
    }

    public Task Disconnect( ) => Connection.StopAsync();

    public IDisposable On<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicProperties )] TSignal>( Func<TSignal, Task> handler )
        where TSignal : Signal<TSignal>
        => Connection.On( typeof( TSignal ).Name, handler );

    public IDisposable On<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicProperties )] TSignal>( Action<TSignal> handler )
        where TSignal : Signal<TSignal>
        => Connection.On( typeof( TSignal ).Name, handler );

    public Task Send<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicProperties )] TSignal>( TSignal signal )
        where TSignal : Signal<TSignal>
        => Connection.SendAsync( typeof( TSignal ).Name, signal );
}