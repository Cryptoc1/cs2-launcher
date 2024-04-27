namespace CS2Launcher.AspNetCore.App.Shared;

/// <summary> Describes the name of icons. </summary>
/// <param name="name"> The name of the icon. </param>
public readonly struct IconName( string name )
{
    public static readonly IconName ChevronRight = new( "chevron_right" );
    public static readonly IconName Circle = new( "circle" );
    public static readonly IconName CloudCircle = new( "cloud_circle" );
    public static readonly IconName Error = new( "error" );
    public static readonly IconName Pending = new( "pending" );
    public static readonly IconName ProgressActivity = new( "progress_activity" );

    /// <inheritdoc/>
    public override string ToString( ) => name;
}

/// <summary> Describes the desired size of an icon. </summary>
public enum IconSize
{
    ExtraSmall,
    Small,
    Base,
    Large,
    ExtraLarge,
    ExtraLarge2,
    ExtraLarge3,
    ExtraLarge4,
    ExtraLarge5,
    ExtraLarge6,
    ExtraLarge7,
    ExtraLarge8,
    ExtraLarge9,
}