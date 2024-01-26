using CS2Launcher.AspNetCore.Launcher;

var builder = CS2LauncherApplication.CreateBuilder( args );

await using var app = builder.Build();
await app.RunAsync();