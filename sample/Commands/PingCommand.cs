using CoreRCON.Extensions.CounterStrike;
using CS2Launcher.AspNetCore.Launcher.Abstractions;
using CS2Launcher.AspNetCore.Launcher.Commands;

namespace CS2Launcher.Sample.Commands;

internal sealed class PingCommand( IDedicatedServerConsole console ) : IChatCommand
{
    public Task Invoke( CommandContext context, CancellationToken cancellation )
        => console.Connect(
            ( client, cancellation ) => client.Say( "pong!", cancellation ),
            cancellation );
}