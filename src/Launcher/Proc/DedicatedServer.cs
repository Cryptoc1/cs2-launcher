using System.Runtime.Versioning;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CS2Launcher.AspNetCore.Launcher.Proc;

internal sealed partial class DedicatedServer(
    IHostApplicationLifetime lifetime,
    ILogger<DedicatedServer> logger,
    IOptions<DedicatedServerOptions> optionsAccessor ) : BackgroundService
{
    private readonly DedicatedServerOptions options = optionsAccessor.Value;

    [SupportedOSPlatform( "linux" )]
    [SupportedOSPlatform( "windows" )]
    protected override async Task ExecuteAsync( CancellationToken cancellation )
    {
        if( !options.Enabled )
        {
            return;
        }

        using( var process = new DedicatedServerProcess( options ) )
        using( cancellation.Register( OnCancellation, process ) )
        {
            if( !process.Start() )
            {
                logger.FailedToStart(
                    process.ExitCode,
                    process.StartInfo.RedirectStandardError ? await process.StandardError.ReadToEndAsync( cancellation ) : "<unavailable>",
                    process.StartInfo.RedirectStandardOutput ? await process.StandardOutput.ReadToEndAsync( cancellation ) : "<unavailable>" );

                lifetime.StopApplication();
                return;
            }

            logger.ServerStarted();
            if( options.RedirectOutput )
            {
                process.OutputDataReceived += ( _, args ) => logger.ServerStdOut( args.Data );
                process.BeginOutputReadLine();
            }

            await process.WaitForExitAsync( cancellation );
            if( process.ExitCode is not 0 )
            {
                // NOTE: throwing an exception will cause systemd to restart the launcher
                throw new DedicatedServerCrashedException( process.ExitCode );
            }

            lifetime.StopApplication();
        }

        static void OnCancellation( object? state )
        {
            if( state is DedicatedServerProcess process && process.IsRunning )
            {
                process.Kill( true );
            }
        };
    }
}

/// <summary> Represents an exception that occurs when the <see cref="DedicatedServerProcess"/> crashes. </summary>
/// <param name="exitCode"> The exit code of the underlying process. </param>
public sealed class DedicatedServerCrashedException( int exitCode ) : Exception( $"{exitCode}" );

internal static partial class DedicatedServerLogging
{
    [LoggerMessage( 10, LogLevel.Debug, "Connected RCON (Host = {hostname}, Version = {version})" )]
    public static partial void ConnectedConsole( this ILogger<DedicatedServer> logger, string hostname, string version );

    [LoggerMessage( -1, LogLevel.Critical, "Failed to Start: {exitCode}\n{stderr}\n{stdout}" )]
    public static partial void FailedToStart( this ILogger<DedicatedServer> logger, int exitCode, string stderr, string stdout );

    [LoggerMessage( 1, LogLevel.Information, "Exited: {exitCode}" )]
    public static partial void ServerExited( this ILogger<DedicatedServer> logger, int exitCode );

    [LoggerMessage( 0, LogLevel.Information, "Started" )]
    public static partial void ServerStarted( this ILogger<DedicatedServer> logger );

    [LoggerMessage( 100, LogLevel.Trace, "{stdout}" )]
    public static partial void ServerStdOut( this ILogger<DedicatedServer> logger, string? stdout );
}