using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CS2Launcher.AspNetCore.Launcher.Infrastructure;

internal static class EndpointExtensions
{
    public static bool IsApiEndpoint( this Endpoint endpoint )
    {
        ArgumentNullException.ThrowIfNull( endpoint );
        return endpoint.Metadata.GetMetadata<ApiControllerAttribute>() is not null;
    }
}