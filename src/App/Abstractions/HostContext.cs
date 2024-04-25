using System.Reflection;

namespace CS2Launcher.AspNetCore.App.Abstractions;

public abstract class HostContext( Assembly host )
{
    private readonly Lazy<string> name = new( ( ) => host.GetName().Name! );

    public string AssemblyName => name.Value;
}