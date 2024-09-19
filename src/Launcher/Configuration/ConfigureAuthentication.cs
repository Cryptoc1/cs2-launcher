using AspNet.Security.OpenId.Steam;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace CS2Launcher.AspNetCore.Launcher.Configuration;

internal sealed class ConfigureAuthentication : IPostConfigureOptions<CookieAuthenticationOptions>, IPostConfigureOptions<SteamAuthenticationOptions>
{
    public void PostConfigure( string? name, CookieAuthenticationOptions options )
    {
        ArgumentNullException.ThrowIfNull( options );
        options.LoginPath = "/login";
    }

    public void PostConfigure( string? name, SteamAuthenticationOptions options )
    {
        ArgumentNullException.ThrowIfNull( options );
        options.SaveTokens = true;
    }
}