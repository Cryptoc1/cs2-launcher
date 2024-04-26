using System.Reflection;

namespace CS2Launcher.AspNetCore.App.Abstractions;

public abstract class HostContext( Assembly host )
{
    public string AssemblyName { get; } = host.GetName().Name!;
}