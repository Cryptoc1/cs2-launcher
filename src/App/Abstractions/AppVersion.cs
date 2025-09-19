using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace CS2Launcher.AspNetCore.App.Abstractions;

/// <summary> Represents the version numbers of a build of the CS2Launcher App. </summary>
[ImmutableObject( true )]
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

    /// <summary> The height of a pre-release in relation to the version. </summary>
    public float? Candidate { get; }

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
        if( !string.IsNullOrEmpty( informational ) )
        {
            var index = informational.IndexOf( '-' );
            if( index > 0 )
            {
                var end = informational.IndexOf( '+' );
                if( end < 0 )
                {
                    end = informational.Length;
                }

                index += 4;
                Candidate = float.Parse( informational[ index..end ], CultureInfo.InvariantCulture );
            }

            index = informational.IndexOf( '+' );
            if( index > 0 )
            {
                Metadata = informational[ (index + 1)..(index + 8) ];
            }
        }
    }

    /// <inheritdoc/>
    public override string ToString( )
    {
        var version = $"{Major}.{Minor}.{Patch}";
        if( Candidate.HasValue )
        {
            version += $"-rc.{Candidate}";
        }

        if( !string.IsNullOrEmpty( Metadata ) )
        {
            version += $"+{Metadata}";
        }

        return version;
    }

    /// <inheritdoc/>
    public static implicit operator string( AppVersion version ) => version.ToString();
}