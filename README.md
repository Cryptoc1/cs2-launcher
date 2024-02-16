# CS2 Launcher

A .NET Generic Host Builder for running CS2 Dedicated Servers.

## Getting Started

- Create a new console project
```shell
dotnet new console -n {launcher-name}
```

- Update the project to target the Web Sdk
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
    <!-- ... -->
</Project>
```

- Add a reference to `CS2Launcher.AspNetCore.Launcher`
```shell
dotnet add package CS2Launcher.AspNetCore.Launcher
```

- Update the `Program.cs`
```csharp
using CS2Launcher.AspNetCore.Launcher;

var builder = CS2LauncherApplication.CreateBuilder( args );

await using var app = builder.Build();
await app.RunAsync();
```

- Update the Server configuration
```json
{
    "Server": {
        "ProcessPriority": "Normal",
        "Program": "C:\\SteamLibrary\\steamapps\\common\\Counter-Strike Global Offensive\\game\\bin\\win64\\cs2.exe",
        "RconPassword": "test",
        "RedirectOutput": "false",
        "WorkshopMapIds": [3070280193]
    }
}
```

Need a proper example? Checkout the [sample](./sample) project.