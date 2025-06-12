using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace CS2Launcher.AspNetCore.App.Components;

/// <summary> Defines a type of component state. </summary>
public abstract record State<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.All )] T>;

/// <summary> Defines an abstract component that reacts to mutation to its <see cref="State"/>. </summary>
/// <typeparam name="T"> The type of <see cref="State{T}"/>. </typeparam>
public abstract class Stateful<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.All )] T> : ComponentBase, IDisposable
    where T : State<T>, new()
{
    private bool disposed;
    private PersistingComponentStateSubscription? persistence;

    [Inject]
    private PersistentComponentState PersistentState { get; init; } = default!;

    /// <summary> The current state values. </summary>
    protected T State { get; private set; } = new();

    /// <inheritdoc/>
    public void Dispose( )
    {
        Dispose( true );
        GC.SuppressFinalize( this );
    }

    /// <inheritdoc/>
    protected virtual void Dispose( bool disposing )
    {
        if( !disposed )
        {
            if( disposing )
            {
                persistence?.Dispose();
            }

            disposed = true;
        }
    }

    /// <summary> Mutate the component's <see cref="State"/>. </summary>
    /// <param name="mutator"> A method that mutates the component's state. </param>
    /// <returns> A task that completes when the component has reacted to the mutation. </returns>
    protected async ValueTask<bool> Mutate( Func<T, ValueTask<T>> mutator )
    {
        ArgumentNullException.ThrowIfNull( mutator );

        var state = await mutator( State );
        if( State != state )
        {
            State = state;
            await InvokeAsync( StateHasChanged );

            return true;
        }

        return false;
    }

    /// <summary> Mutate the component's <see cref="State"/>. </summary>
    /// <param name="mutator"> A method that mutates the component's state. </param>
    /// <returns> A task that completes when the component has reacted to the mutation. </returns>
    protected async Task<bool> Mutate( Func<T, Task<T>> mutator )
    {
        ArgumentNullException.ThrowIfNull( mutator );

        var state = await mutator( State );
        if( State != state )
        {
            State = state;
            await InvokeAsync( StateHasChanged );

            return true;
        }

        return false;
    }

    /// <summary> Mutate the component's <see cref="State"/>. </summary>
    /// <param name="mutator"> A method that mutates the component's state. </param>
    /// <returns> A task that completes when the component has reacted to the mutation. </returns>
    protected async ValueTask<bool> Mutate( Func<T, T> mutator )
    {
        ArgumentNullException.ThrowIfNull( mutator );

        var state = mutator( State );
        if( State != state )
        {
            State = state;
            await InvokeAsync( StateHasChanged );

            return true;
        }

        return false;
    }

    /// <summary> Mutate the component's <see cref="State"/>. </summary>
    /// <param name="mutator"> A method that mutates the component's state. </param>
    /// <returns> A task that completes when the component has reacted to the mutation. </returns>
    protected async ValueTask<bool> Mutate( Func<T, IAsyncEnumerable<T>> mutator )
    {
        ArgumentNullException.ThrowIfNull( mutator );

        var mutated = false;
        await foreach( var state in mutator( State ) )
        {
            if( State != state )
            {
                mutated = true;

                State = state;
                await InvokeAsync( StateHasChanged );
            }
        }

        return mutated;
    }

    /// <summary> Mutate the component's <see cref="State"/>. </summary>
    /// <param name="mutator"> A method that mutates the component's state. </param>
    /// <returns> A task that completes when the component has reacted to the mutation. </returns>
    protected async ValueTask<bool> Mutate( Func<T, IEnumerable<T>> mutator )
    {
        ArgumentNullException.ThrowIfNull( mutator );

        var mutated = false;
        foreach( var state in mutator( State ) )
        {
            if( State != state )
            {
                mutated = true;

                State = state;
                await InvokeAsync( StateHasChanged );
            }
        }

        return mutated;
    }

    /// <summary> Attempt to restore the component's <see cref="State"/> from <see cref="PersistentComponentState"/>. </summary>
    /// <param name="key"> The key to restore state from. </param>
    /// <returns> A value indicating whether a persisted state was restored. </returns>
    [UnconditionalSuppressMessage( "Trimming", "IL2026", Justification = "The generic type parameter 'T' is properly annotated to prevent trimming of metadata required by serialization." )]
    protected bool TryRestoreFromPersistence( string? key = default )
    {
        key = $"{GetType().FullName}_{key}";
        if( PersistentState.TryTakeFromJson<T>( key, out var state ) )
        {
            State = state ?? new();
            return true;
        }

        persistence ??= PersistentState.RegisterOnPersisting( ( ) =>
        {
            PersistentState.PersistAsJson( key, State );
            return Task.CompletedTask;
        } );

        return false;
    }
}