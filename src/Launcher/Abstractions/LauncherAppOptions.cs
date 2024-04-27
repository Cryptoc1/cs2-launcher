namespace CS2Launcher.AspNetCore.Launcher.Abstractions;

/// <summary> Represents the options for configuring the CS2Launcher App. </summary>
public sealed class LauncherAppOptions
{
    /// <summary> The Steam64 User IDs of users permitted access. </summary>
    public List<ulong> Users { get; set; } = [];
}