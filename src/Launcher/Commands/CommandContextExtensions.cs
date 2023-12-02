using CoreRCON.Extensions.CounterStrike;
using CS2Launcher.AspNetCore.Launcher.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CS2Launcher.AspNetCore.Launcher.Commands;

public static class CommandContextExtensions
{
    /// <summary> Determines whether the command was invoked by an administrator. </summary>
    /// <seealso cref="CommandsOptions.AdminIds"/>
    public static bool IsAdmin( this CommandContext context )
    {
        ArgumentNullException.ThrowIfNull( context );
        if( context.Chat.Player?.Name is null or "Console" ) return false;

        var options = context.Services.GetRequiredService<IOptions<CommandsOptions>>();
        return options.Value.AdminIds.Contains(
            context.Chat.Player.SteamId );
    }

    public static Task Reply( this CommandContext context, string text, CancellationToken cancellation = default )
    {
        ArgumentNullException.ThrowIfNull( context );

        var console = context.Services.GetRequiredService<IDedicatedServerConsole>();
        return console.Connect(
            ( client, cancellation ) => client.Say( text, cancellation ),
            cancellation );
    }
}