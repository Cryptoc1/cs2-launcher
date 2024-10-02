using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace CS2Launcher.AspNetCore.Launcher.Abstractions;

/// <summary> Represents the options of the underlying dedicated server process. </summary>
[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicProperties )]
public sealed class DedicatedServerOptions
{
    /// <summary> The name of a cfg to auto-exec. </summary>
    public string? AutoExec { get; set; }

    /// <summary> Additional custom arguments. </summary>
    public List<string> AdditionalArgs { get; set; } = [];

    /// <summary> Whether the server is enabled. </summary>
    public bool Enabled { get; set; } = true;

    /// <summary> The <c>game_alias</c> to launch with. </summary>
    public string GameAlias { get; set; } = "deathmatch";

    /// <summary> A Game Server Login Token to launch with. </summary>
    public string? GSLToken { get; set; }

    /// <summary> The host ip to bind the server to. </summary>
    [Required]
    public string Host { get; set; }

    /// <summary> Whether to launch the dedicated server in 'insecure' mode. </summary>
    public bool Insecure { get; set; }

    /// <summary> The name of the map to launch with. </summary>
    [Required]
    public string Map { get; set; } = "<empty>";

    /// <summary> A callback that is invoked when arguments are being built, prior to launching the dedicated server process. </summary>
    public Action<CS2ArgumentsBuilder>? OnBuildArguments { get; set; }

    /// <summary> The priorty of the underlying process. </summary>
    /// <remarks> Only supported by Windows. </remarks>
    public ProcessPriorityClass ProcessPriority { get; set; } = ProcessPriorityClass.Normal;

    /// <summary> The absolute path of the CS2 program to launch. </summary>
    [Required]
    public string Program { get; set; }

    /// <summary> The RCON password the launch with. </summary>
    public string? RconPassword { get; set; }

    /// <summary> Whether stdout of the underlying process should be redirected. </summary>
    public bool RedirectOutput { get; set; } = true;

    /// <summary> The name of the system user to run the underlying process as. </summary>
    public string? SystemUser { get; set; }

    /// <summary> The absolute path of the directory to run the underlying process in. </summary>
    public string? WorkingDirectory { get; set; }

    /// <summary> The id of a workshop map collection to launch with. </summary>
    public ulong? WorkshopCollectionId { get; set; }

    /// <summary> The ids of workshop maps to launch with. </summary>
    /// <remarks> Use <see cref="Map"/> to configure the default map of the server. </remarks>
    public List<ulong> WorkshopMapIds { get; set; } = [];
}