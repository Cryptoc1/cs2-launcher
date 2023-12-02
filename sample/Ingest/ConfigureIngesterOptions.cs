using CS2Launcher.AspNetCore.Launcher.Ingest;
using Microsoft.Extensions.Options;

namespace CS2Launcher.Sample.Ingest;

internal sealed class ConfigureIngester : IConfigureOptions<IngesterOptions>
{
    public void Configure( IngesterOptions options )
        => options.Parsers.Add(
            IngestParser.Create( new ConsoleMessageParser() ) );
}