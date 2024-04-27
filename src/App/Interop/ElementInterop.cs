using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CS2Launcher.AspNetCore.App.Interop;

internal sealed class ElementInterop( IJSRuntime runtime ) : Interop( runtime, "Element" )
{
    public ValueTask FocusAndSelectEnd( ElementReference element ) => Access( module => module.InvokeVoidAsync( "focusAndSelectEnd", element ) );
}