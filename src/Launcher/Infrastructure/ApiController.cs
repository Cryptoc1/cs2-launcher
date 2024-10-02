using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CS2Launcher.AspNetCore.Launcher.Infrastructure;

/// <summary> Base controller for CS2Launcher APIs. </summary>
[ApiController]
[Produces( MediaTypeNames.Application.Json )]
[ProducesResponseType( StatusCodes.Status400BadRequest )]
public abstract class ApiController : ControllerBase;