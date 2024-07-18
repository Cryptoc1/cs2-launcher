using System.Diagnostics.CodeAnalysis;
using Microsoft.JSInterop;

namespace CS2Launcher.AspNetCore.App.Interop;

internal sealed class LocalStorageInterop( IJSRuntime runtime ) : Interop( runtime, "LocalStorage" )
{
    public ValueTask<T?> Get<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.All )] T>( string key, CancellationToken cancellation = default )
        => Access( module => module.InvokeAsync<T?>( "get", cancellation, key ) );

    public ValueTask Set<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.All )] T>( string key, T value, CancellationToken cancellation = default )
    {
        ArgumentException.ThrowIfNullOrEmpty( key );
        ArgumentNullException.ThrowIfNull( value );

        return Access( module => module.InvokeVoidAsync( "set", cancellation, key, value ) );
    }
}