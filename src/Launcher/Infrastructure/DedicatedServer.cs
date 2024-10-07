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
    private readonly SemaphoreSlim processLock = new( 1, 1 );
    private DedicatedServerProcess? process;

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
        var options = optionsAccessor.Value;
        if( !options.Enabled )
        {
            return Task.CompletedTask;
        }

        if( !(process = new DedicatedServerProcess( options, cancellation )).Start() )
        {
            logger.FailedToStart( process.ExitCode );
            return Task.CompletedTask;
        }

        logger.ServerStarted();
        if( options.RedirectOutput )
        {
            process.OutputDataReceived += ( _, args ) => logger.ServerStdOut( args.Data );
            process.BeginOutputReadLine();
        }

        return process.WaitForExitAsync( cancellation );
    }

    public async ValueTask<ServerMetrics> Metrics( )
    {
        if( process is null || !await Refresh() )
        {
            return ServerMetrics.Zero;
        }

        var status = AsServerStatus( process );
        if( status is not (ServerStatus.Starting or ServerStatus.Running) )
        {
            return new( ServerMemoryMetrics.Zero, ServerProcessorMetrics.Zero, status );
        }

        return new(
            new( process.PagedMemorySize64, process.VirtualMemorySize64, process.WorkingSet64 )
            {
                Peak = new( process.PeakPagedMemorySize64, process.PeakVirtualMemorySize64, process.PeakWorkingSet64 )
            },
            new( process.PrivilegedProcessorTime, process.TotalProcessorTime, process.UserProcessorTime ),
            status );
    }

    private async ValueTask<bool> Refresh( CancellationToken cancellation = default )
    {
        if( !optionsAccessor.Value.Enabled || process is null )
        {
            return false;
        }

        await processLock.WaitAsync( cancellation );
        try
        {
            process.Refresh();
            return true;
        }
        finally
        {
            processLock.Release();
        }
    }

    public async ValueTask<ServerStatus> Status( )
    {
        if( process is null || !await Refresh() )
        {
            return ServerStatus.NotStarted;
        }

        return AsServerStatus( process );
    }

    private static ServerStatus AsServerStatus( DedicatedServerProcess process )
    {
        ArgumentNullException.ThrowIfNull( process );
        return process switch
        {
            not null and { IsRunning: true } => ServerStatus.Running,
            not null and { HasExited: true } => ServerStatus.Crashed,
            not null => ServerStatus.Starting,
            null => ServerStatus.NotStarted,
        };
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