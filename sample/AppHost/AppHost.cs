using CS2Launcher.Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder( args );

builder.AddCS2Launcher<Projects.Launcher>( "launcher" );

await using var app = builder.Build();
await app.RunAsync();