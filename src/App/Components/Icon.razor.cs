namespace CS2Launcher.AspNetCore.App.Components;

/// <summary> Describes the name of icons. </summary>
/// <param name="name"> The name of the icon. </param>
public readonly struct IconName( string name )
{
    public static readonly IconName ChevronRight = new( "chevron_right" );
    public static readonly IconName CopyAll = new( "copy_all" );
    public static readonly IconName Circle = new( "circle" );
    public static readonly IconName CloudCircle = new( "cloud_circle" );
    public static readonly IconName Help = new( "help" );
    public static readonly IconName Error = new( "error" );
    public static readonly IconName Info = new( "info" );
    public static readonly IconName Map = new( "map" );
    public static readonly IconName Memory = new( "memory" );
    public static readonly IconName MemoryAlt = new( "memory_alt" );
    public static readonly IconName Pace = new( "pace" );
    public static readonly IconName Pending = new( "pending" );
    public static readonly IconName ProgressActivity = new( "progress_activity" );
    public static readonly IconName Report = new( "report" );
    public static readonly IconName Terminal = new( "terminal" );
    public static readonly IconName Warning = new( "warning" );

    /// <inheritdoc/>
    public override string ToString( ) => name;
}