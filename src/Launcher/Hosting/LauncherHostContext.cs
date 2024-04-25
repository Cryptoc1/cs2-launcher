using System.Reflection;
using CS2Launcher.AspNetCore.App.Abstractions;

namespace CS2Launcher.AspNetCore.Launcher.Hosting;

public sealed class LauncherHostContext( Assembly assembly ) : HostContext( assembly );