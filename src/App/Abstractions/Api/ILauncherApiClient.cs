namespace CS2Launcher.AspNetCore.App.Abstractions.Api;

/// <summary> Defines a client for invoking CS2Launcher App APIs. </summary>
public interface ILauncherApiClient
{
    /// <summary> Provides access to Server API operations. </summary>
    IServerApi Server { get; }
}