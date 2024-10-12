using System.Diagnostics;
using CS2Launcher.AspNetCore.Launcher.Abstractions;

namespace CS2Launcher.AspNetCore.Launcher.Infrastructure;

internal sealed class DedicatedServerProcess : IAsyncDisposable
{
    private readonly SemaphoreSlim locker = new( 1, 1 );
    private readonly ProcessPriorityClass priority;
    private readonly Process process;

    public int ExitCode => process.ExitCode;
    public bool HasExited => process.HasExited;
    public bool IsRunning => IsStarted && !HasExited;
    public bool IsStarted { get; private set; }

    public long PagedMemory => process.PagedMemorySize64;
    public long PeakPagedMemory => process.PeakPagedMemorySize64;

    public long VirtualMemory => process.VirtualMemorySize64;
    public long PeakVirtualMemory => process.PeakVirtualMemorySize64;

    public long WorkingMemory => process.WorkingSet64;
    public long PeakWorkingMemory => process.PeakWorkingSet64;

    public TimeSpan PrivilegedProcessorTime => process.PrivilegedProcessorTime;
    public TimeSpan TotalProcessorTime => process.TotalProcessorTime;
    public TimeSpan UserProcessorTime => process.UserProcessorTime;

    public int ThreadCount => process.Threads.Count;

    public DedicatedServerProcess( DedicatedServerOptions options )
    {
        priority = options.ProcessPriority;
        process = new()
        {
            EnableRaisingEvents = false,
            StartInfo = new( options.Program )
            {
                Arguments = BuildArguments( options ),
                CreateNoWindow = true,
                UseShellExecute = false,
                UserName = options.SystemUser,
                WindowStyle = ProcessWindowStyle.Minimized,
                WorkingDirectory = !string.IsNullOrEmpty( options.WorkingDirectory ) ? Path.GetFullPath( options.WorkingDirectory ) : default
            }
        };
    }

    private static string BuildArguments( DedicatedServerOptions options )
    {
        var arguments = new CS2ArgumentsBuilder( "-dedicated" )
            .Append( options.Insecure ? "-insecure" : string.Empty );

        if( !string.IsNullOrEmpty( options.GSLToken ) )
        {
            arguments.Append( $"+sv_setsteamaccount {options.GSLToken}" );
        }

        if( !string.IsNullOrWhiteSpace( options.RconPassword ) )
        {
            arguments.Append( @$"+con_enable true +rcon_password ""{options.RconPassword}""" );
        }

        if( options.WorkshopCollectionId.HasValue )
        {
            arguments.Append( $"+host_workshop_collection {options.WorkshopCollectionId.Value}" );
        }

        foreach( var workshopMapId in options.WorkshopMapIds )
        {
            arguments.Append( $"+host_workshop_map {workshopMapId}" );
        }

        arguments.Append( $"+map {options.Map}" )
            .Append( !string.IsNullOrEmpty( options.GameAlias ) ? $"+game_alias {options.GameAlias}" : string.Empty )
            .Append( !string.IsNullOrEmpty( options.AutoExec ) ? $"+exec {options.AutoExec}" : string.Empty )
            .Append( options.AdditionalArgs );

        options.OnBuildArguments?.Invoke( arguments );
        return arguments.Build();
    }

    public async ValueTask DisposeAsync( )
    {
        await Terminate();

        locker.Dispose();
        process.Dispose();

        GC.SuppressFinalize( this );
    }

    public async ValueTask Refresh( CancellationToken cancellation = default )
    {
        if( IsRunning )
        {
            await locker.WaitAsync( cancellation );
            try
            {
                process.Refresh();
            }
            finally
            {
                locker.Release();
            }
        }
    }

    public async ValueTask<bool> Start( CancellationToken cancellation )
    {
        if( IsRunning )
        {
            await Refresh( cancellation );
            return true;
        }

        await locker.WaitAsync( cancellation );
        try
        {
            if( IsStarted = process.Start() )
            {
                process.PriorityBoostEnabled = true;
                if( OperatingSystem.IsWindows() )
                {
                    process.PriorityClass = priority;
                }
            }
        }
        finally
        {
            locker.Release();
        }

        await Refresh( cancellation );
        return IsStarted;
    }

    public async ValueTask Terminate( CancellationToken cancellation = default )
    {
        if( IsRunning )
        {
            await locker.WaitAsync( cancellation );
            try
            {
                process.Kill( true );
            }
            finally
            {
                locker.Release();
            }
        }
    }

    public Task WaitForExit( CancellationToken cancellation = default ) => process.WaitForExitAsync( cancellation );
}