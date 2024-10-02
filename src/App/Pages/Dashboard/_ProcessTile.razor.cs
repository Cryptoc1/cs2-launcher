using CS2Launcher.AspNetCore.App.Abstractions.Api;
using CS2Launcher.AspNetCore.App.Components;

namespace CS2Launcher.AspNetCore.App.Pages.Dashboard;

public sealed record ProcessTileState : State
{
    public bool IsLoading { get; init; } = true;
    public ServerStatus Status { get; init; }

    internal static async Task<ProcessTileState> Load( IServerApi serverApi, ProcessTileState state )
    {
        ArgumentNullException.ThrowIfNull( serverApi );
        ArgumentNullException.ThrowIfNull( state );

        return state with
        {
            IsLoading = false,
            Status = await serverApi.Status()
        };
    }
}