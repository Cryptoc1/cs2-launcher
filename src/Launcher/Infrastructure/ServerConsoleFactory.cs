using System.Net;
using CoreRCON;
using CS2Launcher.AspNetCore.Launcher.Abstractions;

namespace CS2Launcher.AspNetCore.Launcher.Infrastructure;

internal sealed class ServerConsoleFactory( IDedicatedServer server ) : IServerConsoleFactory
{
    public RCONClient Create( )
    {
        var options = server.Options;
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