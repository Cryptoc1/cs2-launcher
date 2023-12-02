using System.Net;
using CoreRCON;
using Microsoft.Extensions.Options;

namespace CS2Launcher.AspNetCore.Launcher.Proc;

internal sealed class DedicatedServerConsole( IOptions<DedicatedServerOptions> options ) : IDedicatedServerConsole
{
    public async Task Connect( Func<RCONClient, CancellationToken, Task> accessor, CancellationToken cancellation )
    {
        ArgumentNullException.ThrowIfNull( accessor );

        using var client = await Connect( options.Value );
        await accessor( client, cancellation );
    }

    private static async Task<RCONClient> Connect( DedicatedServerOptions options )
    {
        var client = new RCONClient(
            string.IsNullOrWhiteSpace( options.Host ) ? IPAddress.Loopback : IPAddress.Parse( options.Host ),
            27015,
            options.RconPassword );

        await client.ConnectAsync();
        return client;
    }
}