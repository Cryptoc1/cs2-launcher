using System.Text;

namespace CS2Launcher.AspNetCore.Launcher.Abstractions;

/// <summary> A builder of CS2 dedicated server arguments. </summary>
public sealed class CS2ArgumentsBuilder
{
    private StringBuilder builder = new();

    /// <summary> Create a new builder. </summary>
    /// <param name="args"> The initial arguments. </param>
    public CS2ArgumentsBuilder( params string[] args ) => Append( args );

    /// <summary> Append the given <paramref name="value"/> to the builder. </summary>
    /// <param name="value"> The argument value to append. </param>
    public CS2ArgumentsBuilder Append( string? value )
    {
        value = value?.Trim();
        if( !string.IsNullOrEmpty( value ) )
        {
            builder = builder.Append( ' ' ).Append( value );
        }

        return this;
    }

    /// <summary> Append the given <paramref name="values"/> to the builder. </summary>
    /// <param name="values"> The argument values to append. </param>
    public CS2ArgumentsBuilder Append( IEnumerable<string?> values ) => values.Aggregate( this, ( args, arg ) => args.Append( arg ) );

    /// <summary> Build the dedicated server arguments string. </summary>
    /// <returns> The combined, normalized, arguments. </returns>
    public string Build( ) => builder.ToString().Trim();
}