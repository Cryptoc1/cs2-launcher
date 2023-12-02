using CS2Launcher.AspNetCore.Launcher.Abstractions;

namespace CS2Launcher.AspNetCore.Launcher.Commands;

internal sealed class CommandDescriptor( string name, CommandHandler handler )
{
    public string Name { get; } = name;
    public CommandHandler Handler { get; } = handler;
}

public delegate Task CommandHandler( CommandContext context, CancellationToken cancellation );