using CS2Launcher.AspNetCore.Launcher.Commands;

namespace CS2Launcher.AspNetCore.Launcher.Abstractions;

public interface IChatCommand
{
    Task Invoke( CommandContext context, CancellationToken cancellation );
}