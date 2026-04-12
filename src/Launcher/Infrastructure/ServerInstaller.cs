using CS2Launcher.AspNetCore.Launcher.Abstractions;
using Microsoft.Extensions.Hosting;

namespace CS2Launcher.AspNetCore.Launcher.Infrastructure;

internal sealed class ServerInstaller : BackgroundService, IServerInstaller
{
    private readonly TaskCompletionSource<ServerInstallerResult> completion = new( TaskCreationOptions.RunContinuationsAsynchronously );

    protected override async Task ExecuteAsync( CancellationToken cancellation )
    {
        await using( cancellation.Register( ( ) => completion.TrySetCanceled( cancellation ) ) )
        {
            var options = new ServerInstallerOptions
            {
                GameDirectory = "/data/cs2-dedicated"
            };

            try
            {
                // var result = await ServerInstallerProcess.Run( options, cancellation );
                // completion.SetResult( result );

                throw new NotImplementedException( "We're getting there..." );
            }
            catch( Exception e )
            {
                completion.TrySetException( e );
            }
        }
    }

    public async ValueTask<ServerInstallerResult> WaitForInstallation( CancellationToken cancellation )
    {
        if( completion.Task.IsCompletedSuccessfully )
        {
            return completion.Task.Result;
        }

        try
        {
            if( completion.Task.IsFaulted )
            {
                throw completion.Task.Exception;
            }

            return await completion.Task.WaitAsync( cancellation );
        }
        catch( Exception e )
        {
            throw new ServerInstallerException( default, e );
        }
    }
}

internal sealed class ServerInstallerException( string? message, Exception? inner = default ) : Exception( message ?? "Server installation failed.", inner );