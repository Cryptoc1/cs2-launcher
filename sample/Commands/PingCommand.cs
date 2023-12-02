using CS2Launcher.AspNetCore.Launcher.Abstractions;
using CS2Launcher.AspNetCore.Launcher.Commands;

namespace CS2Launcher.Sample.Commands;

internal sealed class PingCommand : IChatCommand
{
    public Task Invoke( CommandContext context, CancellationToken cancellation )
        => context.Reply( "pong!", cancellation );
}