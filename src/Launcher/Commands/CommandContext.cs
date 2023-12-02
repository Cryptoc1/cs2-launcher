using CoreRCON.Parsers.Standard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CS2Launcher.AspNetCore.Launcher.Commands;

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

    /// <summary> Determines whether the command was invoked by an administrator. </summary>
    /// <seealso cref="CommandsOptions.AdminIds"/>
    public bool IsAdmin( )
    {
        if( Chat.Player?.Name is null or "Console" ) return false;

        var options = Services.GetRequiredService<IOptions<CommandsOptions>>();
        return options.Value.AdminIds.Contains( Chat.Player.SteamId );
    }
}