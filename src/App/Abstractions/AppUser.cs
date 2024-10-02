namespace CS2Launcher.AspNetCore.App.Abstractions;

/// <summary> Represents a User of the App. </summary>
/// <param name="AvatarUrl"> Url to the user's avatar image. </param>
/// <param name="Name"> The user's display name. </param>
public sealed record AppUser( Uri AvatarUrl, string Name );