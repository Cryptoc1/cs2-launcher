using CS2Launcher.AspNetCore.App.Hosting;
using CS2Launcher.Sample.App;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault( args );
builder.UseCS2Launcher<AppRoot>();

await using var app = builder.Build();
await app.RunAsync();