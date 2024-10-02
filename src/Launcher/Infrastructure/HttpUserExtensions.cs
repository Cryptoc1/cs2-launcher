using System.Security.Claims;
using CS2Launcher.AspNetCore.App.Abstractions;
using CS2Launcher.AspNetCore.Launcher.Abstractions;
using Microsoft.AspNetCore.Http;

namespace CS2Launcher.AspNetCore.Launcher.Infrastructure;

internal static class HttpUserExtensions
{
    private static readonly object UserKey = new();

    public static AppUser? GetUser( this HttpContext context )
    {
        ArgumentNullException.ThrowIfNull( context );

        if( context.User.Identity?.IsAuthenticated is not true )
        {
            return default;
        }

        if( context.Items.TryGetValue( UserKey, out var value ) && value is AppUser user )
        {
            return user;
        }

        context.Items[ UserKey ] = user = new(
            new( context.User.FindFirstValue( LauncherClaimTypes.Avatar )! ),
            context.User.Identity.Name! );

        return user;
    }
}