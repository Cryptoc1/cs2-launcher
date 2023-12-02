using System.Globalization;
using System.Text.RegularExpressions;
using CoreRCON.Parsers.Abstractions;
using CoreRCON.Parsers.Standard;

namespace CS2Launcher.Sample.Ingest;

internal sealed class ConsoleMessageParser : RegexParser<ChatMessage>
{
    public ConsoleMessageParser( ) : base( @"(?<Sender>""Console<(?<ClientId>\d+?)>"") (?<Channel>say_team|say) ""(?<Message>.+?)""" )
    {
    }

    protected override ChatMessage Convert( GroupCollection groups ) => new(
        groups[ "Channel" ].Value is "say" ? MessageChannel.All : MessageChannel.Team,
        groups[ "Message" ].Value,
        new( int.Parse( groups[ "ClientId" ].Value, CultureInfo.InvariantCulture ), "Console", string.Empty, string.Empty )
    );
}