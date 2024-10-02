using System.Net;
using System.Net.Http.Json;
using CS2Launcher.AspNetCore.App.Abstractions.Api;
using CS2Launcher.AspNetCore.App.Json;

namespace CS2Launcher.AspNetCore.App.Infrastructure;

internal sealed class ApiProblemHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync( HttpRequestMessage request, CancellationToken cancellation )
    {
        var response = await base.SendAsync( request, cancellation );
        try
        {
            return response.EnsureSuccessStatusCode();
        }
        catch( HttpRequestException exception )
        {
            if( response.StatusCode >= HttpStatusCode.BadRequest )
            {
                var problem = await response.Content.ReadFromJsonAsync( AppJsonContext.Default.ApiProblem, cancellation );
                throw new ApiProblemException( exception, problem! );
            }
        }

        return response;
    }
}

/// <summary> Represents an exception that occurs when the Middleware API returns a <c>400: Bad Request</c> response. </summary>
/// <param name="inner"> The original request exception. </param>
/// <param name="problem"> The deserialized Problem Details. </param>
public sealed class ApiProblemException( HttpRequestException inner, ApiProblem problem ) : HttpRequestException( inner.Message, inner, inner.StatusCode )
{
    /// <summary> The <see cref="ApiProblem"/> returned by the Middleware API. </summary>
    public ApiProblem Problem { get; } = problem;
}