using System.Reflection;

namespace CS2Launcher.AspNetCore.Launcher.Hosting;

/// <summary> Represents metadata about the launcher host. </summary>
public sealed class LauncherHostContext
{
    /// <summary> The name of the launcher host's assembly. </summary>
    public string AssemblyName { get; }

    private LauncherHostContext( string assemblyName ) => AssemblyName = assemblyName;

    /// <summary> Create a <see cref="LauncherHostContext"/> from the given <paramref name="assembly"/>. </summary>
    public static LauncherHostContext From( Assembly assembly )
    {
        ArgumentNullException.ThrowIfNull( assembly );
        return new( assembly.GetName().Name! );
    }
}