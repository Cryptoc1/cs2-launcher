using System.ComponentModel.DataAnnotations;

namespace CS2Launcher.AspNetCore.Launcher.Abstractions;

internal sealed record class ServerInstallerOptions
{
    public string? Beta { get; set; }

    [Required]
    public string GameDirectory { get; set; }

    public bool Validate { get; set; }
}