using AspNet.Security.OpenId.Steam;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CS2Launcher.AspNetCore.Launcher.Controllers;

public sealed class AppController : Controller
{
    [Authorize]
    public ViewResult Index( ) => View( "_App" );

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