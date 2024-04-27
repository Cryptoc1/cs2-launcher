using System.Globalization;
using System.Security.Claims;
using AspNet.Security.OpenId.Steam;
using CS2Launcher.AspNetCore.Launcher.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace CS2Launcher.AspNetCore.Launcher.Authorization;

internal sealed class AppUserAuthorizationHandler( IOptions<LauncherAppOptions> optionsAccessor ) : AuthorizationHandler<AppUserRequirement>
{
    protected override Task HandleRequirementAsync( AuthorizationHandlerContext context, AppUserRequirement requirement )
    {
        ArgumentNullException.ThrowIfNull( context );
        ArgumentNullException.ThrowIfNull( requirement );

        if( context.User.TryGetSteamUserId( out var userId ) && optionsAccessor.Value.Users.Contains( userId ) )
        {
            context.Succeed( requirement );
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }
}

internal sealed class AppUserRequirement : IAuthorizationRequirement;

internal static class SteamClaimsExtensions
{
    public static bool TryGetSteamUserId( this ClaimsPrincipal principal, out ulong userId )
    {
        ArgumentNullException.ThrowIfNull( principal );
        if( principal.Identity?.IsAuthenticated is true )
        {
            var value = principal.FindFirstValue( ClaimTypes.NameIdentifier );
            if( value?.StartsWith( SteamAuthenticationConstants.Namespaces.Identifier, StringComparison.OrdinalIgnoreCase ) is true )
            {
                return ulong.TryParse( value[ SteamAuthenticationConstants.Namespaces.Identifier.Length.. ], CultureInfo.InvariantCulture, out userId );
            }
        }

        userId = default;
        return false;
    }
}