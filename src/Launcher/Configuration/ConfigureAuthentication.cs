using System.Security.Claims;
using System.Text.Json;
using AspNet.Security.OpenId.Steam;
using CS2Launcher.AspNetCore.Launcher.Abstractions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace CS2Launcher.AspNetCore.Launcher.Configuration;

internal sealed class ConfigureAuthentication( IOptions<SteamApiOptions> steamApiOptions ) : IPostConfigureOptions<CookieAuthenticationOptions>, IPostConfigureOptions<SteamAuthenticationOptions>
{
    public void PostConfigure( string? name, CookieAuthenticationOptions options )
    {
        ArgumentNullException.ThrowIfNull( options );
        options.LoginPath = "/login";
    }

    public void PostConfigure( string? name, SteamAuthenticationOptions options )
    {
        ArgumentNullException.ThrowIfNull( options );

        options.ApplicationKey = steamApiOptions.Value.ApiKey;
        options.SaveTokens = true;

        options.Events.OnAuthenticated = context =>
        {
            var profile = context.UserPayload?.RootElement.GetProperty( SteamAuthenticationConstants.Parameters.Response )
                .GetProperty( SteamAuthenticationConstants.Parameters.Players )
                .EnumerateArray()
                .FirstOrDefault();

            if( profile?.ValueKind is JsonValueKind.Object && profile.Value.TryGetProperty( "avatarfull", out var avatar ) )
            {
                context.Identity?.AddClaim(
                    new( LauncherClaimTypes.Avatar, avatar.GetString()!, ClaimValueTypes.String, context.Options.ClaimsIssuer ) );
            }

            return Task.CompletedTask;
        };
    }
}