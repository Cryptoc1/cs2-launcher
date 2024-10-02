using CS2Launcher.AspNetCore.Launcher;
using CS2Launcher.Sample.App;

var builder = CS2LauncherApplication.CreateBuilder( args )
    .WithLauncherApp<AppRoot>();

await using var app = builder.Build();
await app.RunAsync();