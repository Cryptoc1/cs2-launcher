using System.Net.Mime;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;

namespace CS2Launcher.AspNetCore.Launcher.Configuration;

internal sealed class ConfigureResponseCompression : IConfigureOptions<ResponseCompressionOptions>
{
    public void Configure( ResponseCompressionOptions options )
    {
        options.EnableForHttps = true;
        options.MimeTypes = [.. ResponseCompressionDefaults.MimeTypes,
			// documents
			MediaTypeNames.Application.Json,
            "text/json",
            MediaTypeNames.Application.Manifest,
            MediaTypeNames.Application.Octet,
            MediaTypeNames.Application.Pdf,

			// images
			MediaTypeNames.Image.Gif,
            MediaTypeNames.Image.Icon,
            MediaTypeNames.Image.Jpeg,
            "image/jpg",
            MediaTypeNames.Image.Png,
            MediaTypeNames.Image.Svg,
            "image/svg",
            MediaTypeNames.Image.Webp,

			// fonts
			"application/vnd.ms-fontobject",
            MediaTypeNames.Font.Otf,
            MediaTypeNames.Font.Ttf,
            MediaTypeNames.Font.Woff,
            MediaTypeNames.Font.Woff2 ];
    }
}