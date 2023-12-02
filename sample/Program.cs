using CS2Launcher.AspNetCore.Launcher;
using CS2Launcher.Sample.Commands;
using CS2Launcher.Sample.Ingest;

var builder = CS2LauncherApplication.CreateBuilder( args );

builder.Services.AddCS2Command<PingCommand>( "ping" );
builder.Services.ConfigureOptions<ConfigureIngester>();

await using var app = builder.Build();
await app.RunAsync();