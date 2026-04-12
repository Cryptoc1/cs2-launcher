namespace CS2Launcher.AspNetCore.Launcher.Abstractions;

/// <summary> Describes a service that manages the installation of the server. </summary>
public interface IServerInstaller
{
    /// <summary> Waits for the server installation to complete. </summary>
    public ValueTask<ServerInstallerResult> WaitForInstallation( CancellationToken cancellation );
}

public sealed record ServerInstallerResult( string Output, int ExitCode, string? Error );