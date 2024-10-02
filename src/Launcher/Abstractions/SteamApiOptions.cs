using System.ComponentModel.DataAnnotations;

namespace CS2Launcher.AspNetCore.Launcher.Abstractions;

/// <summary> Represents options for calling Steam WebApis. </summary>
public sealed class SteamApiOptions
{
    /// <summary> The api key to be used for authentication. </summary>
    [Required]
    public string ApiKey { get; set; }
}