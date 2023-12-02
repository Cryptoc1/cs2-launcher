using CoreRCON;

namespace CS2Launcher.AspNetCore.Launcher.Abstractions;

public interface IDedicatedServerConsole
{
    Task Connect( Func<RCONClient, CancellationToken, Task> accessor, CancellationToken cancellation );
}