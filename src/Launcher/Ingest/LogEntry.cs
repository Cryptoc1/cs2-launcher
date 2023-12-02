using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CS2Launcher.AspNetCore.Launcher.Ingest;

internal sealed partial record LogEntry( string Body, DateTime Timestamp )
{
    [GeneratedRegex( @"^(\d{2}/\d{2}/\d{4} - \d{2}:\d{2}:\d{2}\.\d{3}) - " )]
    private static partial Regex Parser( );

    public static bool TryParse( string? value, [NotNullWhen( true )] out LogEntry? entry )
    {
        if( !string.IsNullOrWhiteSpace( value ) )
        {
            var match = Parser().Match( value );
            if( match.Success && DateTime.TryParseExact( match.Groups[ 1 ].Value, "MM/dd/yyyy - HH:mm:ss.fff", CultureInfo.InvariantCulture, default, out var timestamp ) )
            {
                var body = value[ match.Groups[ 0 ].Length.. ].Trim();
                if( body.Length is not 0 )
                {
                    entry = new( body, timestamp );
                    return true;
                }
            }
        }

        entry = default;
        return false;
    }
}