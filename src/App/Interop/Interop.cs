using Microsoft.JSInterop;
using CS2Launcher.AspNetCore.App.Infrastructure;
using Nito.AsyncEx;

namespace CS2Launcher.AspNetCore.App.Interop;

internal abstract class Interop( IJSRuntime runtime, string moduleName ) : IAsyncDisposable
{
    public string ModulePath => $"/_content/CS2Launcher.AspNetCore.App/Interop/{moduleName}.module.js";

    private readonly AsyncLock moduleLock = new();

    private IJSObjectReference? module;
    protected IJSRuntime Runtime { get; } = runtime;

    protected async ValueTask Access( Func<IJSObjectReference, ValueTask> method )
    {
        PlatformGuard.ThrowIfNotBrowser();

        await EnsureModuleReference();
        await method( module! );
    }

    protected async ValueTask<T> Access<T>( Func<IJSObjectReference, ValueTask<T>> method )
    {
        PlatformGuard.ThrowIfNotBrowser();

        await EnsureModuleReference();
        return await method( module! );
    }

    public async ValueTask DisposeAsync( )
    {
        if( module is not null )
        {
            await module.DisposeAsync();
        }

        GC.SuppressFinalize( this );
    }

    private async ValueTask EnsureModuleReference( )
    {
        if( module is not null ) return;

        using( await moduleLock.LockAsync() )
        {
            module ??= await Runtime.InvokeAsync<IJSObjectReference>( "import", ModulePath );
        }
    }
}