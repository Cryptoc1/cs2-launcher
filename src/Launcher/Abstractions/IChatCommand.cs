using CoreRCON.Parsers.Standard;

namespace CS2Launcher.AspNetCore.Launcher.Abstractions;

public interface IChatCommand
{
    Task Invoke( CommandContext context, CancellationToken cancellation );
}

public sealed class CommandContext
{
    public string[] Arguments { get; }
    public ChatMessage Chat { get; }
    public string Command { get; }
    public IServiceProvider Services { get; }

    internal CommandContext( string[] arguments, ChatMessage chat, string command, IServiceProvider services )
    {
        Arguments = arguments;
        Chat = chat;
        Command = command;
        Services = services;
    }
}