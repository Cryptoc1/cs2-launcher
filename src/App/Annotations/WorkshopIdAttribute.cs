using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace CS2Launcher.AspNetCore.App.Annotations;

/// <summary> A <see cref="ValidationAttribute"/> for Workshop Url/Ids. </summary>
public sealed class WorkshopIdAttribute( ) : ValidationAttribute( "The {0} field is not a valid Workshop Url or Workshop Id." )
{
    /// <inheritdoc/>
    public override bool IsValid( object? value )
    {
        if( value is ulong )
        {
            return true;
        }

        if( value is not string str )
        {
            return false;
        }

        if( ulong.TryParse( str, out var _ ) )
        {
            return true;
        }

        return WorkshopIdParser.Match( str );
    }
}

/// <summary> Utility class for parsing a Workshop FileId from an artibtrary string value. </summary>
public static partial class WorkshopIdParser
{
    [GeneratedRegex( @"^((http(s)?:\/\/)?steamcommunity.com\/sharedfiles\/filedetails\/?\?id=(?<id>\d+))$", RegexOptions.IgnoreCase | RegexOptions.Singleline )]
    private static partial Regex Parser( );

    /// <summary> Determine whether the given value is a valid Workshop FileId, or Workshop Url. </summary>
    /// <param name="value"> The value to match. </param>
    public static bool Match( string value )
    {
        if( string.IsNullOrWhiteSpace( value ) )
        {
            return false;
        }

        return Parser().IsMatch( value );
    }

    /// <summary> Attempt to parse the Workshop FileId from the given <paramref name="value"/>. </summary>
    /// <param name="value"> The value to parse. </param>
    /// <param name="workshopId"> The Workshop File Id. </param>
    /// <remarks> Given <paramref name="value"/> can be a string representation of a <see cref="long"/> or <see cref="ulong"/>, or a Steam Workshop File Url (e.g. <c>https://steamcommunity.com/sharedfiles/filedetails/?id={workshopId}</c>) </remarks>
    public static bool TryParseId( string value, [NotNullWhen( true )] out ulong workshopId )
    {
        if( !string.IsNullOrWhiteSpace( value ) )
        {
            if( ulong.TryParse( value, out workshopId ) )
            {
                return true;
            }

            var matches = Parser().Match( value );
            if( matches.Success && ulong.TryParse( matches.Groups[ "id" ].Value, out workshopId ) )
            {
                return true;
            }
        }

        workshopId = default;
        return false;
    }
}