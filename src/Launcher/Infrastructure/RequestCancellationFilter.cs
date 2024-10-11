using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CS2Launcher.AspNetCore.Launcher.Infrastructure;

internal sealed class RequestCancellationFilter : IExceptionFilter
{
    public void OnException( ExceptionContext context )
    {
        ArgumentNullException.ThrowIfNull( context );

        if( context.HttpContext.RequestAborted.IsCancellationRequested && IsCancellationException( context.Exception ) )
        {
            context.ExceptionHandled = true;
            context.Result = new StatusCodeResult( StatusCodes.Status499ClientClosedRequest );
        }
    }

    private static bool IsCancellationException( Exception? exception )
    {
        if( exception is null )
        {
            return false;
        }

        if( exception switch
        {
            ConnectionAbortedException or ConnectionResetException => true,
            IOException io => io.Message is "The client reset the request stream.",
            OperationCanceledException or TaskCanceledException => true,

            _ => false
        } )
        {
            return true;
        }

        return IsCancellationException( exception?.InnerException );
    }
}