using CS2Launcher.AspNetCore.Launcher;
using CS2Launcher.AspNetCore.Launcher.Proc;

var builder = CS2LauncherApplication.CreateBuilder( args );
builder.Services.Configure<DedicatedServerOptions>( options => options.Enabled = false );

await using var app = builder.Build();
await app.RunAsync();