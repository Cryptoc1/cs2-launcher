using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace CS2Launcher.AspNetCore.Launcher.Infrastructure;

internal static class RequestCancellationMiddleware
{
    public static IApplicationBuilder UseRequestCancellation( this IApplicationBuilder app )
    {
        ArgumentNullException.ThrowIfNull( app );

        return app.Use( async ( context, next ) =>
        {
            try
            {
                await next();
            }
            catch( OperationCanceledException e ) when( e.CancellationToken == context.RequestAborted && !context.Response.HasStarted )
            {
                context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;

                var endpoint = context.GetEndpoint();
                if( endpoint?.IsApiEndpoint() is true )
                {
                    await context.RequestServices.GetRequiredService<IProblemDetailsService>().WriteAsync( new()
                    {
                        AdditionalMetadata = endpoint.Metadata,
                        Exception = e,
                        HttpContext = context,
                        ProblemDetails = context.RequestServices.GetRequiredService<ProblemDetailsFactory>()
                            .CreateProblemDetails( context, context.Response.StatusCode )
                    } );
                }
            }
        } );
    }
}