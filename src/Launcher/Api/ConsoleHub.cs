using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CS2Launcher.AspNetCore.Launcher.Api;

[Authorize]
public sealed class ConsoleHub : Hub<IConsoleCaller>
{
    /// <inheritdoc/>
    public override Task OnConnectedAsync( )
    {
        return Task.CompletedTask;
    }
}

public interface IConsoleCaller;