using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using CS2Launcher.AspNetCore.Launcher.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Racoon;
using Racoon.Extensions.CounterStrike.Parsers;

namespace CS2Launcher.AspNetCore.Launcher.Infrastructure;

internal sealed class ServerConsoleFactory(
    ILogger<ServerConsoleFactory> logger,
    IOptions<DedicatedServerOptions> optionsAccessor ) : IServerConsoleFactory
{
    public async ValueTask<RCONClient> Create( CancellationToken cancellation )
    {
        var options = optionsAccessor.Value;

        var host = await ResolveServerHost( options, cancellation );
        logger.OnResolvedHost( options.Host, host );

        return new( host, 27015, options.RconPassword!, new()
        {
            AutoConnect = true,
            OnCreatingParserPool = builder => builder.UseCounterStrike()
        } );
    }

    private static async ValueTask<IPAddress> ResolveServerHost( DedicatedServerOptions options, CancellationToken cancellation )
    {
        ArgumentNullException.ThrowIfNull( options );

        if( !string.IsNullOrWhiteSpace( options.Host ) )
        {
            if( options.Host.Equals( "localhost", StringComparison.OrdinalIgnoreCase ) )
            {
                return IPAddress.Loopback;
            }

            if( IPAddress.TryParse( options.Host, out var address ) )
            {
                return address;
            }

            var addresses = await Dns.GetHostAddressesAsync( options.Host, cancellation );
            return addresses.First( address => address.AddressFamily is AddressFamily.InterNetwork );
        }

        foreach( var adapter in NetworkInterface.GetAllNetworkInterfaces() )
        {
            if( !adapter.Supports( NetworkInterfaceComponent.IPv4 ) )
            {
                continue;
            }

            if( adapter.OperationalStatus is OperationalStatus.Up && IsCandidateInterface( adapter.NetworkInterfaceType ) )
            {
                var properties = adapter.GetIPProperties();
                if( properties.GatewayAddresses.Any( gateway => gateway.Address.AddressFamily is AddressFamily.InterNetwork ) )
                {
                    foreach( var info in properties.UnicastAddresses )
                    {
                        if( info.Address.AddressFamily is AddressFamily.InterNetwork && info.Address != IPAddress.Loopback )
                        {
                            return info.Address;
                        }
                    }
                }
            }
        }

        throw new InvalidOperationException( "The host of the Dedicated Server could not be resolved." );

        static bool IsCandidateInterface( NetworkInterfaceType type ) => type is not NetworkInterfaceType.Loopback and (NetworkInterfaceType.Ethernet or NetworkInterfaceType.Ethernet3Megabit or NetworkInterfaceType.FastEthernetFx or NetworkInterfaceType.FastEthernetT or NetworkInterfaceType.GigabitEthernet);
    }
}

internal static partial class ServerConsoleFactoryLogging
{
    [LoggerMessage( 0, LogLevel.Information, "Resolved dedicated server host '{host}' to: {address}" )]
    public static partial void OnResolvedHost( this ILogger<ServerConsoleFactory> logger, string? host, IPAddress address );
}