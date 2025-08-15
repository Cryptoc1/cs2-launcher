using System.Net;
using CS2Launcher.AspNetCore.Launcher.Abstractions;
using Microsoft.Extensions.Options;
using Racoon;
using Racoon.Extensions.CounterStrike.Parsers;

namespace CS2Launcher.AspNetCore.Launcher.Infrastructure;

internal sealed class ServerConsoleFactory( IOptions<DedicatedServerOptions> optionsAccessor ) : IServerConsoleFactory
{
    public RCONClient Create( )
    {
        var options = optionsAccessor.Value;

        // TODO: make port configurable
        return new( IPAddress.Parse( options.Host ), 27015, options.RconPassword!, new()
        {
            AutoConnect = true,
            OnCreatingParserPool = builder => builder.UseCounterStrike()
        } );
    }
}