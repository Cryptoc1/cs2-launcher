namespace CS2Launcher.AspNetCore.App.Infrastructure;

internal static class PlatformGuard
{
    public static void ThrowIfNotBrowser( string? message = null )
    {
        if( !OperatingSystem.IsBrowser() )
        {
            throw new PlatformNotSupportedException( message );
        }
    }
}