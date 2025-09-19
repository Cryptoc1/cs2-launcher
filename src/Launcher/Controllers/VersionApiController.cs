using CS2Launcher.AspNetCore.App.Abstractions;
using CS2Launcher.AspNetCore.Launcher.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace CS2Launcher.AspNetCore.Launcher.Controllers;

/// <summary> Endpoints for the API version. </summary>
public sealed class VersionApiController : ApiController
{
    /// <summary> Retrieve the current version of the application. </summary>
    [HttpGet( "api/version" )]
    public ActionResult<AppVersion> Index( ) => Ok( AppVersion.Value );
}