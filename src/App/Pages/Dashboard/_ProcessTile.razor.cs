using CS2Launcher.AspNetCore.App.Abstractions.Api;
using ESCd.AspNetCore.Components.Stateful;

namespace CS2Launcher.AspNetCore.App.Pages.Dashboard;

public sealed record ProcessTileState : State<ProcessTileState>
{
    public bool IsLoading { get; init; } = true;
    public ServerStatus Status { get; init; }

    internal static async Task<ProcessTileState> Load( IServerApi serverApi, ProcessTileState state, CancellationToken cancellation )
    {
        ArgumentNullException.ThrowIfNull( serverApi );
        ArgumentNullException.ThrowIfNull( state );

        return state with
        {
            IsLoading = false,
            Status = await serverApi.Status( cancellation )
        };
    }
}