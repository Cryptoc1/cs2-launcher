using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace CS2Launcher.AspNetCore.Launcher.Proc;

[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicProperties )]
public sealed class DedicatedServerOptions
{
    public string? AutoExec { get; set; }

    public List<string> AdditionalArgs { get; set; } = [];

    public string GameAlias { get; set; } = "deathmatch";

    public string? GLSToken { get; set; }

    public string? Host { get; set; }

    public bool Insecure { get; set; }

    [Required]
    public string Map { get; set; } = "<empty>";

    public ProcessPriorityClass ProcessPriority { get; set; } = ProcessPriorityClass.Normal;

    [Required]
    public string Program { get; set; }

    public bool RedirectOutput { get; set; } = true;

    public string? RconPassword { get; set; }

    public string? User { get; set; }

    public string? WorkingDirectory { get; set; }

    public ulong? WorkshopCollectionId { get; set; }

    public List<ulong> WorkshopMapIds { get; set; } = [];
}