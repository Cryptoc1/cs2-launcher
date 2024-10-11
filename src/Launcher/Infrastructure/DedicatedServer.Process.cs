using System.Diagnostics;
using System.Text;
using CS2Launcher.AspNetCore.Launcher.Abstractions;

namespace CS2Launcher.AspNetCore.Launcher.Infrastructure;

internal sealed class DedicatedServerProcess : Process, IAsyncDisposable
{
    public bool IsRunning => IsStarted && !HasExited;
    public bool IsStarted { get; private set; }

    private readonly CancellationTokenRegistration cancellation;
    private readonly SemaphoreSlim locker = new( 1, 1 );
    private readonly ProcessPriorityClass priority;

    public DedicatedServerProcess( DedicatedServerOptions options, CancellationToken cancellation )
    {
        this.cancellation = cancellation.Register( OnCancellation, this );
        priority = options.ProcessPriority;

        EnableRaisingEvents = false;
        StartInfo = new( options.Program )
        {
            Arguments = BuildArguments( options ),
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Minimized
        };

        if( options.RedirectOutput )
        {
            StartInfo.RedirectStandardError = true;
            StartInfo.RedirectStandardOutput = true;
            StartInfo.StandardErrorEncoding = Encoding.UTF8;
            StartInfo.StandardOutputEncoding = Encoding.UTF8;
            StartInfo.UseShellExecute = false;
        }

        if( !string.IsNullOrEmpty( options.SystemUser ) )
        {
            StartInfo.UserName = options.SystemUser;
        }

        if( !string.IsNullOrEmpty( options.WorkingDirectory ) )
        {
            StartInfo.WorkingDirectory = Path.GetFullPath( options.WorkingDirectory );
        }

        static void OnCancellation( object? state )
        {
            if( state is DedicatedServerProcess process && process.IsRunning )
            {
                process.Kill( true );
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

    protected override void Dispose( bool disposing )
    {
        if( disposing )
        {
            if( IsRunning )
            {
                // NOTE: ensure child processes are disposed
                Kill( true );
            }

            cancellation.Dispose();
            locker.Dispose();
        }

        base.Dispose( disposing );
    }

    public async ValueTask DisposeAsync( )
    {
        await cancellation.DisposeAsync();

        locker.Dispose();
        base.Dispose( true );

        GC.SuppressFinalize( this );
    }

    public async ValueTask<bool> Refresh( CancellationToken cancellation = default )
    {
        if( !IsRunning )
        {
            return false;
        }

        await locker.WaitAsync( cancellation );
        try
        {
            base.Refresh();
            return true;
        }
        finally
        {
            locker.Release();
        }
    }

    public new bool Start( )
    {
        if( IsStarted = base.Start() )
        {
            PriorityBoostEnabled = true;
            if( OperatingSystem.IsWindows() )
            {
                PriorityClass = priority;
            }
        }

        return IsStarted;
    }
}