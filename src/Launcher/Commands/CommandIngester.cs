using System.Collections.Immutable;
using CoreRCON.Parsers.Standard;
using CS2Launcher.AspNetCore.Launcher.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CS2Launcher.AspNetCore.Launcher.Commands;

internal sealed class CommandIngester( IOptions<CommandsOptions> optionsAccessor, IServiceProvider serviceProvider ) : IIngestHandler<ChatMessage>
{
    private readonly Dictionary<string, CommandDescriptor> descriptorByName = serviceProvider.GetServices<CommandDescriptor>()
        .ToDictionary( descriptor => descriptor.Name, StringComparer.OrdinalIgnoreCase );

    public async Task OnIngested( ChatMessage chat, CancellationToken cancellation )
    {
        var message = chat.Message?.Trim();
        if( string.IsNullOrWhiteSpace( message ) ) return;

        var options = optionsAccessor.Value;
        if( message[ 0 ] != options.Token ) return;

        var index = message.IndexOf( ' ', 1 );
        var command = index > 1 ? message[ 1..index ] : message[ 1.. ];

        if( descriptorByName.TryGetValue( command, out var descriptor ) )
        {
            var arguments = message[ (command.Length + 1).. ].Split( ' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries );
            var context = new CommandContext(
                arguments,
                chat,
                command,
                serviceProvider );

            await descriptor.Handler( context, cancellation );
        }
    }
}