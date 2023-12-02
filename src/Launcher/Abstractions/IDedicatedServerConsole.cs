using CoreRCON;

public interface IDedicatedServerConsole
{
    Task Connect( Func<RCONClient, CancellationToken, Task> accessor, CancellationToken cancellation );
}