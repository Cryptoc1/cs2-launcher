using CoreRCON.Parsers.Abstractions;

namespace CS2Launcher.AspNetCore.Launcher.Abstractions;

public interface IIngestHandler<T>
    where T : class, IParseable<T>
{
    Task OnIngested( T value, CancellationToken cancellation );
}