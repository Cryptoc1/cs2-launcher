using System.Reflection;

namespace CS2Launcher.AspNetCore.Launcher.Hosting;

/// <summary> Represents metadata about the launcher host. </summary>
/// <param name="AssemblyName"> The name of the launcher host's assembly. </param>
public sealed record LauncherHostContext( string AssemblyName )
{
    /// <summary> Create a <see cref="LauncherHostContext"/> from the given <paramref name="assembly"/>. </summary>
    public static LauncherHostContext From( Assembly assembly )
    {
        ArgumentNullException.ThrowIfNull( assembly );
        return new( assembly.GetName().Name! );
    }
}