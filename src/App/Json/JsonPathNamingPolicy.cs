using System.Text.Json;

namespace CS2Launcher.AspNetCore.App.Json;

/// <inheritdoc />
public sealed class JsonPathNamingPolicy( JsonNamingPolicy policy ) : JsonNamingPolicy
{
    /// <inheritdoc />
    public override string ConvertName( string name ) => string.Join(
        '.',
        name.Split( '.' ).Select( policy.ConvertName ) );
}