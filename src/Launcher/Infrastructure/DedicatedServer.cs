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
    private readonly SemaphoreSlim locker = new( 0, 1 );
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

        base.Dispose();
        GC.SuppressFinalize( this );
    }

    protected override async Task ExecuteAsync( CancellationToken cancellation )
    {
        var options = optionsAccessor.Value;
        if( !options.Enabled )
        {
            return;
        }

        if( !(process ??= new DedicatedServerProcess( options, cancellation )).Start() )
        {
            logger.FailedToStart( process.ExitCode );
            return;
        }

        logger.ServerStarted();
        if( options.RedirectOutput )
        {
            process.OutputDataReceived += ( _, args ) => logger.ServerStdOut( args.Data );
            process.BeginOutputReadLine();
        }

        await process.WaitForExitAsync( cancellation );
        await locker.WaitAsync( cancellation );

        await StartAsync( cancellation );
    }

    public async ValueTask<ServerMetrics> Metrics( CancellationToken cancellation )
    {
        if( process is null || !await process.Refresh( cancellation ) )
        {
            return ServerMetrics.Zero;
        }

        return new(
            new( process.PagedMemorySize64, process.VirtualMemorySize64, process.WorkingSet64 )
            {
                Peak = new( process.PeakPagedMemorySize64, process.PeakVirtualMemorySize64, process.PeakWorkingSet64 )
            },
            new( process.PrivilegedProcessorTime, process.TotalProcessorTime, process.UserProcessorTime ) );
    }

    public void Restart( )
    {
        if( process?.IsRunning is true )
        {
            process.Kill( true );
        }

        locker.Release();
    }

    public async ValueTask<ServerStatus> Status( CancellationToken cancellation )
    {
        if( !optionsAccessor.Value.Enabled )
        {
            return ServerStatus.Disabled;
        }

        if( process is null )
        {
            return ServerStatus.NotStarted;
        }

        await process.Refresh( cancellation );
        return process switch
        {
            not null and { IsRunning: true } => ServerStatus.Running,
            not null and { HasExited: true, ExitCode: -1 } => ServerStatus.Terminated,
            not null and { HasExited: true } => ServerStatus.Crashed,
            not null => ServerStatus.Starting,
            null => ServerStatus.NotStarted,
        };
    }

    public void Terminate( )
    {
        if( process is null )
        {
            return;
        }

        if( process.IsRunning )
        {
            process.Kill( true );
        }
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