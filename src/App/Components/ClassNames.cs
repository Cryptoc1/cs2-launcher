namespace CS2Launcher.AspNetCore.App.Components;

/// <summary> Helpers for combining css class names. </summary>
public static class ClassNames
{
    /// <summary> Combine the given <paramref name="classNames"/>. </summary>
    public static string Combine( params string[] classNames ) => string.Join( " ", classNames.Select( className => className.Trim() ) ).TrimEnd();

    /// <summary> Combine the given <paramref name="classNames"/> with the <c>class</c> value in the given <paramref name="attributes"/>. </summary>
    public static string Combine( IReadOnlyDictionary<string, object>? attributes, params string[] classNames )
    {
        if( attributes?.TryGetValue( "class", out var value ) is true && value is string @class )
        {
            return Combine( [ .. classNames.Append( @class ) ] );
        }

        return Combine( classNames );
    }
}
