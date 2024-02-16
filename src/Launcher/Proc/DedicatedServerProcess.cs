using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text;

namespace CS2Launcher.AspNetCore.Launcher.Proc;

internal sealed class DedicatedServerProcess : Process
{
    public bool IsRunning => IsStarted && !HasExited;
    public bool IsStarted { get; private set; }

    private readonly ProcessPriorityClass priority;

    public DedicatedServerProcess( DedicatedServerOptions options )
    {
        priority = options.ProcessPriority;

        EnableRaisingEvents = false;
        StartInfo = new( options.Program )
        {
            Arguments = BuildArguments( options ),
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Minimized
        };

        if( options.RedirectOutput )
        {
            StartInfo.RedirectStandardError = true;
            StartInfo.RedirectStandardOutput = true;
            StartInfo.StandardErrorEncoding = Encoding.UTF8;
            StartInfo.StandardOutputEncoding = Encoding.UTF8;
            StartInfo.UseShellExecute = false;
        }

        if( !string.IsNullOrEmpty( options.SystemUser ) )
        {
            StartInfo.UserName = options.SystemUser;
        }

        if( !string.IsNullOrEmpty( options.WorkingDirectory ) )
        {
            StartInfo.WorkingDirectory = Path.GetFullPath( options.WorkingDirectory );
        }
    }

    private static string BuildArguments( DedicatedServerOptions options )
    {
        var arguments = new CS2ArgumentsBuilder( "-dedicated" )
            .Append( options.Insecure ? "-insecure" : string.Empty );

        if( !string.IsNullOrEmpty( options.GSLToken ) ) arguments.Append( $"+sv_setsteamaccount {options.GSLToken}" );
        if( !string.IsNullOrWhiteSpace( options.RconPassword ) ) arguments.Append( @$"+con_enable true +rcon_password ""{options.RconPassword}""" );

        if( options.WorkshopCollectionId.HasValue ) arguments.Append( $"+host_workshop_collection {options.WorkshopCollectionId.Value}" );
        foreach( var workshopMapId in options.WorkshopMapIds ) arguments.Append( $"+host_workshop_map {workshopMapId}" );

        arguments.Append( $"+map {options.Map}" )
            .Append( !string.IsNullOrEmpty( options.GameAlias ) ? $"+game_alias {options.GameAlias}" : string.Empty )
            .Append( !string.IsNullOrEmpty( options.AutoExec ) ? $"+exec {options.AutoExec}" : string.Empty )
            .Append( options.AdditionalArgs );

        options.OnBuildArguments?.Invoke( arguments );
        return arguments.Build();
    }

    [SupportedOSPlatform( "linux" )]
    [SupportedOSPlatform( "windows" )]
    public new bool Start( )
    {
        if( IsStarted = base.Start() )
        {
            PriorityBoostEnabled = true;
            if( OperatingSystem.IsWindows() )
            {
                PriorityClass = priority;
            }
        }

        return IsStarted;
    }
}

/// <summary> A builder of CS2 dedicated server arguments. </summary>
public sealed class CS2ArgumentsBuilder
{
    private StringBuilder builder = new();

    /// <summary> Create a new builder. </summary>
    /// <param name="args"> The initial arguments. </param>
    public CS2ArgumentsBuilder( params string[] args ) => Append( args );

    /// <summary> Append the given <paramref name="value"/> to the builder. </summary>
    /// <param name="value"> The argument value to append. </param>
    public CS2ArgumentsBuilder Append( string? value )
    {
        value = value?.Trim();
        if( !string.IsNullOrEmpty( value ) )
        {
            builder = builder.Append( ' ' ).Append( value );
        }

        return this;
    }

    /// <summary> Append the given <paramref name="values"/> to the builder. </summary>
    /// <param name="values"> The argument values to append. </param>
    public CS2ArgumentsBuilder Append( IEnumerable<string?> values ) => values.Aggregate( this, ( args, arg ) => args.Append( arg ) );

    /// <summary> Build the dedicated server arguments string. </summary>
    /// <returns> The combined, normalized, arguments. </returns>
    public string Build( ) => builder.ToString().Trim();
}