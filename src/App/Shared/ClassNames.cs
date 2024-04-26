namespace CS2Launcher.AspNetCore.App.Shared;

public static class ClassNames
{
    public static string Combine( params string[] classNames ) => string.Join( " ", classNames.Select( className => className.Trim() ) ).TrimEnd();

    public static string Combine( IReadOnlyDictionary<string, object>? attributes, params string[] classNames )
    {
        if( attributes?.TryGetValue( "class", out var value ) is true && value is string @class )
        {
            return Combine( [ .. classNames.Append( @class ) ] );
        }

        return Combine( classNames );
    }
}
