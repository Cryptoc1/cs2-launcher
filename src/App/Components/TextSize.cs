namespace CS2Launcher.AspNetCore.App.Components;

/// <summary> Represents Tailwind Text Size classes. </summary>
public enum TextSize
{
    /// <summary> <c>text-xs</c> </summary>
    ExtraSmall,

    /// <summary> <c>text-sm</c> </summary>
    Small,

    /// <summary> <c>text-base</c> </summary>
    Base,

    /// <summary> <c>text-lg</c> </summary>
    Large,

    /// <summary> <c>text-xl</c> </summary>
    ExtraLarge,

    /// <summary> <c>text-2xl</c> </summary>
    ExtraLarge2,

    /// <summary> <c>text-3xl</c> </summary>
    ExtraLarge3,

    /// <summary> <c>text-4xl</c> </summary>
    ExtraLarge4,

    /// <summary> <c>text-5xl</c> </summary>
    ExtraLarge5,

    /// <summary> <c>text-6xl</c> </summary>
    ExtraLarge6,

    /// <summary> <c>text-7xl</c> </summary>
    ExtraLarge7,

    /// <summary> <c>text-8xl</c> </summary>
    ExtraLarge8,

    /// <summary> <c>text-9xl</c> </summary>
    ExtraLarge9,

    /// <summary> <c>text-10xl</c> </summary>
    ExtraLarge10,
}

internal static class TextSizeExtensions
{
    public static TextSize Scale( this TextSize size, int scale )
        => ( TextSize )Math.Max(
            0,
            Math.Min( ( int )size + scale, 12 ) );

    public static string ToCssClass( this TextSize size ) => size switch
    {
        TextSize.ExtraSmall => "text-xs",
        TextSize.Small => "text-sm",
        TextSize.Base => "text-base",
        TextSize.Large => "text-lg",
        TextSize.ExtraLarge => "text-xl",
        TextSize.ExtraLarge2 => "text-2xl",
        TextSize.ExtraLarge3 => "text-3xl",
        TextSize.ExtraLarge4 => "text-4xl",
        TextSize.ExtraLarge5 => "text-5xl",
        TextSize.ExtraLarge6 => "text-6xl",
        TextSize.ExtraLarge7 => "text-7xl",
        TextSize.ExtraLarge8 => "text-8xl",
        TextSize.ExtraLarge9 => "text-9xl",
        TextSize.ExtraLarge10 => "text-10xl",
        _ => ""
    };
}