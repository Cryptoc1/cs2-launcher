using AspNet.Security.OpenId.Steam;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CS2Launcher.AspNetCore.Launcher.Controllers;

/// <summary> Default controller for routing to the CS2Launcher App. </summary>
public sealed class AppController : Controller
{
    /// <summary> Default route for the CS2Launcher App. </summary>
    [Authorize]
    public ViewResult Index( ) => View( "_App" );

    /// <summary> Challenges authentication via Steam. </summary>
    [AllowAnonymous]
    [HttpGet( "/login" )]
    public ChallengeResult Login( string returnUrl = "/" ) => Challenge(
        new AuthenticationProperties
        {
            AllowRefresh = true,
            IsPersistent = true,
            RedirectUri = returnUrl,
        },
        SteamAuthenticationDefaults.AuthenticationScheme );
}