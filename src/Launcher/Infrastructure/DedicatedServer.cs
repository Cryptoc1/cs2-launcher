using CS2Launcher.AspNetCore.App.Abstractions.Api;
using CS2Launcher.AspNetCore.Launcher.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CS2Launcher.AspNetCore.Launcher.Infrastructure;

internal sealed partial class DedicatedServer(
    ILogger<DedicatedServer> logger,
    IOptions<DedicatedServerOptions> optionsAccessor ) : BackgroundService, IAsyncDisposable, IDedicatedServer
{
    private DedicatedServerProcess? process;

    public DedicatedServerOptions Options => optionsAccessor.Value;
    public ServerStatus Status => process switch
    {
        not null and { IsRunning: true } => ServerStatus.Running,
        not null and { HasExited: true } => ServerStatus.Crashed,
        not null => ServerStatus.Starting,
        null => ServerStatus.NotStarted,
    };

    public override void Dispose( )
    {
        base.Dispose();
        if( process is not null )
        {
            process.Dispose();
            process = default;
        }

        GC.SuppressFinalize( this );
    }

    public async ValueTask DisposeAsync( )
    {
        if( process is not null )
        {
            await process.DisposeAsync();
            process = default;
        }

        GC.SuppressFinalize( this );
    }

    protected override Task ExecuteAsync( CancellationToken cancellation )
    {
        if( !Options.Enabled )
        {
            return Task.CompletedTask;
        }

        if( !(process = new DedicatedServerProcess( Options, cancellation )).Start() )
        {
            logger.FailedToStart( process.ExitCode );
            return Task.CompletedTask;
        }

        logger.ServerStarted();
        if( Options.RedirectOutput )
        {
            process.OutputDataReceived += ( _, args ) => logger.ServerStdOut( args.Data );
            process.BeginOutputReadLine();
        }

        return process.WaitForExitAsync( cancellation );
    }

    public ServerMetrics GetMetrics( )
    {
        if( !Options.Enabled || process?.HasExited is null or true )
        {
            return ServerMetrics.Zero;
        }

        process.Refresh();
        return new(
            new( process.PagedMemorySize64, process.VirtualMemorySize64, process.WorkingSet64 )
            {
                Peak = new( process.PeakPagedMemorySize64, process.PeakVirtualMemorySize64, process.PeakWorkingSet64 )
            },
            new( process.PrivilegedProcessorTime, process.TotalProcessorTime, process.UserProcessorTime ) );
    }
}

/// <summary> Represents an exception that occurs when the <see cref="DedicatedServerProcess"/> crashes. </summary>
/// <param name="exitCode"> The exit code of the underlying process. </param>
public sealed class DedicatedServerCrashedException( int exitCode ) : Exception( $"{exitCode}" );

internal static partial class DedicatedServerLogging
{
    [LoggerMessage( -1, LogLevel.Critical, "Failed to Start: {exitCode}" )]
    public static partial void FailedToStart( this ILogger<DedicatedServer> logger, int exitCode );

    [LoggerMessage( 0, LogLevel.Information, "Started" )]
    public static partial void ServerStarted( this ILogger<DedicatedServer> logger );

    [LoggerMessage( 100, LogLevel.Trace, "{stdout}" )]
    public static partial void ServerStdOut( this ILogger<DedicatedServer> logger, string? stdout );
}