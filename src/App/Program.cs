using CS2Launcher.AspNetCore.App;
using CS2Launcher.AspNetCore.App.Abstractions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault( args );
builder.Logging.SetMinimumLevel( builder.HostEnvironment.IsDevelopment() ? LogLevel.Information : LogLevel.Error );

builder.Services.AddCS2LauncherApp();

await using var app = builder.Build();
Console.WriteLine( $"CS2Launcher v{AppVersion.Value}" );

await app.RunAsync();
