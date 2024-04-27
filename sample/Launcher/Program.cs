using CS2Launcher.AspNetCore.Launcher;
using CS2Launcher.AspNetCore.Launcher.Proc;
using CS2Launcher.Sample.App;

var builder = CS2LauncherApplication.CreateBuilder( args )
    .WithLauncherApp<AppRoot>();

builder.Services.Configure<DedicatedServerOptions>( options => options.Enabled = false );

await using var app = builder.Build();
await app.RunAsync();