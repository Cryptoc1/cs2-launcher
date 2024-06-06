using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CS2Launcher.AspNetCore.Launcher.Configuration;

internal sealed class ConfigureCookiePolicy : IConfigureOptions<CookiePolicyOptions>
{
    public void Configure( CookiePolicyOptions options )
    {
        ArgumentNullException.ThrowIfNull( options );

        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
    }
}