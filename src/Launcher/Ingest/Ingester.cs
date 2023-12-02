using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CS2Launcher.AspNetCore.Launcher.Ingest;

internal sealed class Ingester( ILogger<Ingester> logger, IOptions<IngesterOptions> options, IngestQueue queue, IServiceProvider serviceProvider ) : BackgroundService
{
    private readonly Dictionary<Type, IngesterDescriptor[]> descriptorsByType = serviceProvider.GetServices<IngesterDescriptor>()
        .GroupBy( descriptor => descriptor.Type )
        .ToDictionary( group => group.Key, group => group.ToArray() );

    protected override async Task ExecuteAsync( CancellationToken cancellation )
    {
        while( !cancellation.IsCancellationRequested )
        {
            var packet = await queue.Dequeue( cancellation );
            if( packet is null ) continue;

            logger.Ingesting( packet );
            foreach( var parser in options.Value.Parsers )
            {
                if( !parser.IsMatch( packet.Body ) )
                {
                    continue;
                }

                var value = parser.Parse( packet.Body );
                logger.ParsedPacket( value );

                if( descriptorsByType.TryGetValue( value.GetType(), out var descriptors ) )
                {
                    try
                    {
                        await Task.WhenAll(
                            descriptors.Select( descriptor => descriptor.Handler( value, cancellation ) ) );
                    }
                    catch( Exception exception )

                        // NOTE: allow cancellations to propagate
                        when( exception is not OperationCanceledException && !(exception is AggregateException aggregate && aggregate.InnerExceptions.OfType<OperationCanceledException>().Any()) )
                    {
                        logger.IngestFailure( exception );
                    }
                }
            }
        }
    }
}

internal static partial class IngesterLogging
{
    [LoggerMessage( 0, LogLevel.Trace, "Ingesting: {packet}" )]
    public static partial void Ingesting( this ILogger<Ingester> logger, LogEntry packet );

    [LoggerMessage( -1, LogLevel.Error, "Unhandled exception while ingesting packet." )]
    public static partial void IngestFailure( this ILogger<Ingester> logger, Exception exception );

    [LoggerMessage( 1, LogLevel.Debug, "Parsed: {value}" )]
    public static partial void ParsedPacket( this ILogger<Ingester> logger, object value );
}