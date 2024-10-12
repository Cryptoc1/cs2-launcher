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
    private readonly SemaphoreSlim restartTrigger = new( 0, 1 );

    private bool disposed;
    private DedicatedServerProcess? process;

    public override void Dispose( )
    {
    }

    public async ValueTask DisposeAsync( )
    {
        if( disposed )
        {
            return;
        }

        base.Dispose();

        if( process is not null )
        {
            await process.DisposeAsync();
            process = default;
        }

        disposed = true;
        GC.SuppressFinalize( this );
    }

    protected override async Task ExecuteAsync( CancellationToken cancellation )
    {
        var options = optionsAccessor.Value;
        if( !options.Enabled )
        {
            return;
        }

        if( await (process ??= new( options )).Start( cancellation ) )
        {
            logger.ServerStarted();
        }
        else
        {
            logger.FailedToStart( process.ExitCode );
        }

        await process.WaitForExit( cancellation );
        await restartTrigger.WaitAsync( cancellation );

        await StartAsync( cancellation );
    }

    public async ValueTask<ServerMetrics> Metrics( CancellationToken cancellation )
    {
        if( process?.IsRunning is not true )
        {
            return ServerMetrics.Zero;
        }

        await process.Refresh( cancellation );
        return new(
            new( process.PagedMemory, process.VirtualMemory, process.WorkingMemory )
            {
                Peak = new( process.PeakPagedMemory, process.PeakVirtualMemory, process.PeakWorkingMemory )
            },
            new( process.ThreadCount, new( process.PrivilegedProcessorTime, process.TotalProcessorTime, process.UserProcessorTime ) ) );
    }

    public async ValueTask Restart( CancellationToken cancellation )
    {
        await Terminate( cancellation );
        restartTrigger.Release();
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

    public async ValueTask Terminate( CancellationToken cancellation )
    {
        if( process?.IsRunning is true )
        {
            await process.Terminate( cancellation );
        }
    }
}

internal static partial class DedicatedServerLogging
{
    [LoggerMessage( -1, LogLevel.Critical, "Failed to Start: {exitCode}" )]
    public static partial void FailedToStart( this ILogger<DedicatedServer> logger, int exitCode );

    [LoggerMessage( 0, LogLevel.Information, "Started" )]
    public static partial void ServerStarted( this ILogger<DedicatedServer> logger );
}