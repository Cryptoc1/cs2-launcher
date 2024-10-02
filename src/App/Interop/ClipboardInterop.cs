using Microsoft.JSInterop;

namespace CS2Launcher.AspNetCore.App.Interop;

internal sealed class ClipboardInterop( IJSRuntime runtime )
{
    public ValueTask Write( string text, CancellationToken cancellation = default )
        => runtime.InvokeVoidAsync( "navigator.clipboard.writeText", cancellation, text );
}