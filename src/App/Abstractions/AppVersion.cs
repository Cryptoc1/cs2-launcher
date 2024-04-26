using System.Reflection;

namespace CS2Launcher.AspNetCore.App.Abstractions;

/// <summary> Represents the version of the CS2Launcher App. </summary>
public sealed class AppVersion
{
    /// <summary> A reference to the current version of the application. </summary>
    public static readonly AppVersion Value = new();

    /// <summary> The major version. </summary>
    public int Major { get; }

    /// <summary> The minor version. </summary>
    public int Minor { get; }

    /// <summary> The patch, or hotfix, version. </summary>
    public int Patch { get; }

    /// <summary> Additional versioning metadata for tracking. </summary>
    public string? Metadata { get; }

    private AppVersion( )
    {
        var assembly = typeof( AppVersion ).Assembly;

        var version = assembly.GetName().Version!;
        Major = version.Major;
        Minor = version.Minor;
        Patch = version.Build;

        var informational = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        var index = informational?.IndexOf( '-' ) ?? -1;
        if( index > 0 )
        {
            Metadata = informational![ (index + 1)..(informational!.IndexOf( '+' ) + 8) ];
        }
    }

    /// <inheritdoc/>
    public override string ToString( )
    {
        var version = $"{Major}.{Minor}.{Patch}";
        return !string.IsNullOrEmpty( Metadata ) ? $"{version}-{Metadata}" : version;
    }

    /// <inheritdoc/>
    public static implicit operator string( AppVersion version ) => version.ToString();
}