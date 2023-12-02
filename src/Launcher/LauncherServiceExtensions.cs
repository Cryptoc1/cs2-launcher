using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using CoreRCON.Parsers.Abstractions;
using CoreRCON.Parsers.Standard;
using CS2Launcher.AspNetCore.Launcher.Abstractions;
using CS2Launcher.AspNetCore.Launcher.Commands;
using CS2Launcher.AspNetCore.Launcher.Ingest;
using CS2Launcher.AspNetCore.Launcher.Proc;
using Microsoft.Extensions.DependencyInjection;

namespace CS2Launcher.AspNetCore.Launcher;

public static class LauncherServiceExtensions
{
    public static IServiceCollection AddCS2Launcher( this IServiceCollection services )
    {
        ArgumentNullException.ThrowIfNull( services );

        services.AddHostedService<DedicatedServer>()
            .AddTransient<IDedicatedServerConsole, DedicatedServerConsole>()
            .AddOptions<DedicatedServerOptions>()
            .BindConfiguration( "Server" )
            .ValidateDataAnnotations();

        services.AddHostedService<Ingester>()
            .AddSingleton( new IngestQueue() )
            .AddOptions<IngesterOptions>()
            .ValidateDataAnnotations();

        services.AddSingleton<CommandIngester>()
            .AddCS2Ingester<ChatMessage, CommandIngester>()
            .AddOptions<CommandsOptions>();

        return services;
    }

    public static IServiceCollection AddCS2Command( this IServiceCollection services, string name, Func<IServiceProvider, CommandHandler> factory )
    {
        ArgumentNullException.ThrowIfNull( services );
        return services.AddSingleton( serviceProvider => new CommandDescriptor( name, factory( serviceProvider ) ) );
    }

    public static IServiceCollection AddCS2Command<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicConstructors )] TCommand>( this IServiceCollection services, string name )
        where TCommand : IChatCommand
    {
        return AddCS2Command( services, name, BuildHandler );

        static CommandHandler BuildHandler( IServiceProvider serviceProvider )
            => ( context, cancellation ) => ActivatorUtilities.GetServiceOrCreateInstance<TCommand>( serviceProvider )
                .Invoke( context, cancellation );
    }

    public static IServiceCollection AddCS2Ingester<T>( this IServiceCollection services, Func<IServiceProvider, IngestHandler> factory )
        where T : class, IParseable<T>
    {
        ArgumentNullException.ThrowIfNull( services );
        return services.AddSingleton( serviceProvider => new IngesterDescriptor( factory( serviceProvider ), typeof( T ) ) );
    }

    public static IServiceCollection AddCS2Ingester<T, [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicConstructors )] TIngester>( this IServiceCollection services )
        where T : class, IParseable<T>
        where TIngester : class, IIngestHandler<T>
    {
        return AddCS2Ingester<T>( services, BuildHandler );

        static IngestHandler BuildHandler( IServiceProvider serviceProvider )
            => ( value, cancellation ) => ActivatorUtilities.GetServiceOrCreateInstance<TIngester>( serviceProvider )
                .OnIngested( Unsafe.As<T>( value ), cancellation );
    }
}