using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CS2Launcher.AspNetCore.Launcher.Ingest;

internal static partial class IngestEndpoint
{
    public static async ValueTask<IResult> Ingest( [FromServices] IngestQueue queue, Stream body, CancellationToken cancellation )
    {
        var value = await ReadAsString( body, cancellation );
        if( !string.IsNullOrWhiteSpace( value ) )
        {
            foreach( var line in Sanitizer().Split( value ) )
            {
                if( LogEntry.TryParse( line, out var packet ) )
                {
                    await queue.Enqueue( packet, cancellation );
                }
            }
        }

        return Results.Ok();
    }

    private static async Task<string?> ReadAsString( Stream source, CancellationToken cancellation )
    {
        using var reader = new StreamReader( source );
        return await reader.ReadToEndAsync( cancellation );
    }

    [GeneratedRegex( "\\r\\n|\\n\\r|\\n|\\r" )]
    private static partial Regex Sanitizer( );
}