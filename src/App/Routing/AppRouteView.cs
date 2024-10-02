using System.Diagnostics.CodeAnalysis;
using CS2Launcher.AspNetCore.App.Abstractions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace CS2Launcher.AspNetCore.App.Routing;

/// <inheritdoc />
public sealed class AppRouteView : RouteView
{
    /// <summary> The application user. </summary>
    [EditorRequired]
    [Parameter]
    public AppUser? User { get; set; }

    /// <inheritdoc />
    [DynamicDependency( DynamicallyAccessedMemberTypes.All, typeof( AppLayout ) )]
    public AppRouteView( ) => DefaultLayout = typeof( AppLayout );

    /// <inheritdoc />
    protected override void Render( RenderTreeBuilder builder )
    {
        builder.OpenComponent<CascadingValue<AppUser?>>( 0 );
        builder.AddComponentParameter( 1, nameof( CascadingValue<AppUser?>.Value ), User );
        builder.AddComponentParameter( 2, nameof( CascadingValue<AppUser?>.ChildContent ), new RenderFragment( builder =>
        {
            builder.OpenComponent<Components.ErrorDialog>( 0 );
            builder.AddComponentParameter( 1, nameof( Components.ErrorDialog.ChildContent ), new RenderFragment( base.Render ) );
            builder.CloseComponent();
        } ) );

        builder.CloseComponent();
    }
}