using CoreRCON.Parsers;
using CoreRCON.Parsers.Abstractions;
using CoreRCON.Parsers.Standard;

namespace CS2Launcher.AspNetCore.Launcher.Ingest;

public sealed class IngesterOptions
{
    public List<IngestParser> Parsers { get; } = [
        IngestParser.Create<ChatMessage>()
    ];
}

public sealed class IngestParser
{
    private readonly Func<string, bool> match;
    private readonly Func<string, object> parse;

    private IngestParser( Func<string, bool> match, Func<string, object> parse )
    {
        this.match = match;
        this.parse = parse;
    }

    /// <summary> Creates an <see cref="IngestParser"/> by resolving a parser from <see cref="ParserPool.Shared"/>. </summary>
    /// <typeparam name="T"> The type of <see cref="IParseable{T}"/>. </typeparam>
    public static IngestParser Create<T>( )
        where T : class, IParseable<T>
        => Create(
            ParserPool.Shared.Get<T>() );

    /// <summary> Creates an <see cref="IngestParser"/> for the given <see cref="IParser{T}"/>. </summary>
    /// <typeparam name="T"> The type of <see cref="IParseable{T}"/>. </typeparam>
    /// <param name="parser"> The parser to create an <see cref="IngestParser"/> for. </param>
    public static IngestParser Create<T>( IParser<T> parser )
        where T : class, IParseable<T>
        => new( parser.IsMatch, parser.Parse );

    public bool IsMatch( string line ) => match( line );

    public object Parse( string line ) => parse( line );
}