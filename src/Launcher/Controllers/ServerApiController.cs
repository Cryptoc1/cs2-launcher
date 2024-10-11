using CS2Launcher.AspNetCore.App.Abstractions.Api;
using CS2Launcher.AspNetCore.Launcher.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace CS2Launcher.AspNetCore.Launcher.Controllers;

/// <summary> Endpoints for the "Server API". </summary>
[Route( "api/server" )]
public sealed class ServerApiController( ILauncherApiClient api ) : ApiController
{
    /// <summary> Request the map to be changed on the server. </summary>
    [HttpPost( "map" )]
    public async Task<ActionResult> ChangeMap( [FromBody] ChangeMapParameters parameters )
    {
        if( !ModelState.IsValid )
        {
            return ValidationProblem( ModelState );
        }

        return Ok(
            await api.Server.ChangeMap( parameters, HttpContext.RequestAborted ) );
    }

    /// <summary> Retrieve performance metrics for the server. </summary>
    [HttpGet( "metrics" )]
    public async Task<ActionResult<ServerMetrics>> Metrics( )
    {
        var metrics = await api.Server.Metrics( HttpContext.RequestAborted );
        return Ok( metrics );
    }

    /// <summary> Restart the server. </summary>
    [HttpPut( "restart" )]
    public async Task<ActionResult<ServerStatus>> Restart( )
    {
        var status = await api.Server.Restart( HttpContext.RequestAborted );
        return Ok( status );
    }

    /// <summary> Retrieve the current status of the server. </summary>
    [HttpGet( "status" )]
    public async Task<ActionResult<ServerStatus>> Status( )
    {
        var status = await api.Server.Status( HttpContext.RequestAborted );
        return Ok( status );
    }

    /// <summary> Terminate the server. </summary>
    [HttpPut( "terminate" )]
    public async Task<ActionResult<ServerStatus>> Terminate( )
    {
        var status = await api.Server.Terminate( HttpContext.RequestAborted );
        return Ok( status );
    }
}