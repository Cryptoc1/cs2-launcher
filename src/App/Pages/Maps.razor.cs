using CS2Launcher.AspNetCore.App.Abstractions.Api;
using CS2Launcher.AspNetCore.App.Components;
using CS2Launcher.AspNetCore.App.Infrastructure;

namespace CS2Launcher.AspNetCore.App.Pages;

public sealed record MapsState : State
{
    public IDictionary<string, string[]>? Errors { get; init; }
    public bool? IsMapChanged { get; init; }
    public bool IsLoading { get; init; } = true;
    public bool IsServerRunning { get; init; }
    public ChangeMapParameters Parameters { get; init; } = new();

    internal static async IAsyncEnumerable<MapsState> ChangeMap( IServerApi serverApi, MapsState state )
    {
        ArgumentNullException.ThrowIfNull( serverApi );
        ArgumentNullException.ThrowIfNull( state );

        yield return state with
        {
            Errors = default,
            IsMapChanged = false,
        };

        yield return await ChangeMap( serverApi, state ) with
        {
            Parameters = new()
        };

        async static Task<MapsState> ChangeMap( IServerApi serverApi, MapsState state )
        {
            try
            {
                return state with
                {
                    Errors = default,
                    IsMapChanged = await serverApi.ChangeMap( state.Parameters )
                };
            }
            catch( ApiProblemException exception )
            {
                return state with
                {
                    Errors = exception.Problem.Errors
                };
            }
        }
    }

    internal static async Task<MapsState> Load( IServerApi serverApi, MapsState state )
    {
        ArgumentNullException.ThrowIfNull( serverApi );
        ArgumentNullException.ThrowIfNull( state );

        return state with
        {
            IsLoading = false,
            IsServerRunning = await serverApi.Status() is ServerStatus.Running
        };
    }
}