using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CS2Launcher.AspNetCore.App.Interop;

internal sealed class DialogInterop( IJSRuntime runtime )
{
    public ValueTask Close( ElementReference element, CancellationToken cancellation = default )
        => runtime.InvokeVoidAsync( "HTMLDialogElement.prototype.close.call", cancellation, element );

    public ValueTask ShowModal( ElementReference element, CancellationToken cancellation = default )
        => runtime.InvokeVoidAsync( "HTMLDialogElement.prototype.showModal.call", cancellation, element );
}