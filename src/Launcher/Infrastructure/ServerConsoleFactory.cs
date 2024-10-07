using System.Net;
using CoreRCON;
using CS2Launcher.AspNetCore.Launcher.Abstractions;
using Microsoft.Extensions.Options;

namespace CS2Launcher.AspNetCore.Launcher.Infrastructure;

internal sealed class ServerConsoleFactory( IOptions<DedicatedServerOptions> optionsAccessor ) : IServerConsoleFactory
{
    public RCONClient Create( )
    {
        var options = optionsAccessor.Value;
        return new(
            IPAddress.Parse( options.Host ),

            // TODO: make port configurable
            27015,
            options.RconPassword!,
            new()
            {
                AutoConnect = true
            } );
    }
}