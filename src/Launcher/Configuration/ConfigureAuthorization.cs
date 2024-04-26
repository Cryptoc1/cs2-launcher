using AspNet.Security.OpenId.Steam;
using CS2Launcher.AspNetCore.Launcher.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace CS2Launcher.AspNetCore.Launcher.Configuration;

internal sealed class ConfigureAuthorization : IConfigureOptions<AuthorizationOptions>
{
    public void Configure( AuthorizationOptions options )
    {
        options.DefaultPolicy = new AuthorizationPolicyBuilder( SteamAuthenticationDefaults.AuthenticationScheme )
            .RequireAuthenticatedUser()
            .AddRequirements( new AppUserRequirement() )
            .Build();
    }
}