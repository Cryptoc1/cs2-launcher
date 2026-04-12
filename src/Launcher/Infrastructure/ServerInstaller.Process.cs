using System.Diagnostics;
using System.Globalization;
using System.Text;
using CS2Launcher.AspNetCore.Launcher.Abstractions;

namespace CS2Launcher.AspNetCore.Launcher.Infrastructure;

internal sealed class ServerInstallerProcess : IAsyncDisposable
{
    private readonly Process process;

    public int ExitCode => process.ExitCode;
    public bool HasExited => process.HasExited;
    public bool IsRunning => IsStarted && !HasExited;
    public bool IsStarted { get; private set; }

    public event Action<string>? OnError;
    public event Action<string>? OnOutput;

    private ServerInstallerProcess( ServerInstallerOptions options )
    {
        process = new()
        {
            EnableRaisingEvents = true,
            StartInfo = new( "steamcmd" )
            {
                Arguments = BuildArguments( options ),
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Minimized,
            }
        };

        process.ErrorDataReceived += OnDataReceived( OnError );
        process.OutputDataReceived += OnDataReceived( OnOutput );

        static DataReceivedEventHandler OnDataReceived( Action<string>? callback ) => ( _, e ) =>
        {
            ArgumentNullException.ThrowIfNull( e );
            if( !string.IsNullOrWhiteSpace( e.Data ) )
            {
                callback?.Invoke( e.Data );
            }
        };
    }

    private static string BuildArguments( ServerInstallerOptions options )
    {
        ArgumentNullException.ThrowIfNull( options );

        var args = new StringBuilder( $"+force_install_dir {options.GameDirectory} +@bClientTryRequestManifestWithoutCode 1 +login anonymous +app_update 730" );
        if( !string.IsNullOrWhiteSpace( options.Beta ) )
        {
            args.Append( CultureInfo.InvariantCulture, $" -beta {options.Beta}" );
        }

        if( options.Validate )
        {
            args.Append( " validate" );
        }

        return args.Append( " +quit" ).ToString();
    }

    public async ValueTask DisposeAsync( )
    {
        if( !process.HasExited )
        {
            process.Kill( true );
            await process.WaitForExitAsync();
        }

        process.Dispose();
    }

    public static async Task<ServerInstallerResult> Run( ServerInstallerOptions options, CancellationToken cancellation = default )
    {
        ArgumentNullException.ThrowIfNull( options );

        await using var process = new ServerInstallerProcess( options );

        var output = new StringBuilder();
        var error = new StringBuilder();

        process.OnError = e => error.AppendLine( e );
        process.OnOutput = e => output.AppendLine( e );

        if( await process.Start() )
        {
            await process.WaitForExit( cancellation );
        }

        return new(
            error.ToString(),
            process.ExitCode,
            output.ToString() );
    }

    public async ValueTask<bool> Start( )
    {
        if( IsRunning )
        {
            return true;
        }

        if( IsStarted = process.Start() )
        {
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            process.PriorityBoostEnabled = true;
            if( OperatingSystem.IsWindows() )
            {
                process.PriorityClass = ProcessPriorityClass.High;
            }
        }

        return IsStarted;
    }

    public async ValueTask WaitForExit( CancellationToken cancellation = default )
    {
        if( process.HasExited )
        {
            return;
        }

        await process.WaitForExitAsync( cancellation ).ConfigureAwait( false );
    }
}