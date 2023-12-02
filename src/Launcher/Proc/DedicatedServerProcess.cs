using System.Diagnostics;
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

        if( !string.IsNullOrEmpty( options.User ) )
        {
            StartInfo.UserName = options.User;
        }

        if( !string.IsNullOrEmpty( options.WorkingDirectory ) )
        {
            StartInfo.WorkingDirectory = Path.GetFullPath( options.WorkingDirectory );
        }
    }

    private static string BuildArguments( DedicatedServerOptions options )
    {
        var arguments = new ArgumentsBuilder( "-dedicated" )
            .Append( options.Insecure ? "-insecure" : string.Empty );

        if( !string.IsNullOrEmpty( options.GLSToken ) ) arguments.Append( $"+sv_setsteamaccount {options.GLSToken}" );
        if( options.WorkshopCollectionId.HasValue ) arguments.Append( $"+host_workshop_collection {options.WorkshopCollectionId.Value}" );
        foreach( var workshopMapId in options.WorkshopMapIds ) arguments.Append( $"+host_workshop_map {workshopMapId}" );

        return arguments.Append( $"+map {options.Map}" )
            .Append( !string.IsNullOrEmpty( options.GameAlias ) ? $"+game_alias {options.GameAlias}" : string.Empty )
            .Append( !string.IsNullOrEmpty( options.AutoExec ) ? $"+exec {options.AutoExec}" : string.Empty )
            .Append( options.AdditionalArgs )
            .Append( @$"+con_enable true +rcon_password ""{options.RconPassword}""" )

            // TODO: allow configuration of server address
            .Append( @"+sv_logecho false +log on +logaddress_add_http ""http://localhost:5000/ingest""" )
            .Build();
    }

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

    private sealed class ArgumentsBuilder
    {
        private StringBuilder builder = new();

        public ArgumentsBuilder( params string[] args ) => Append( args );

        public ArgumentsBuilder Append( string? value )
        {
            value = value?.Trim();
            if( !string.IsNullOrEmpty( value ) )
            {
                builder = builder.Append( ' ' ).Append( value );
            }

            return this;
        }

        public ArgumentsBuilder Append( IEnumerable<string?> values ) => values.Aggregate( this, ( args, arg ) => args.Append( arg ) );

        public string Build( ) => builder.ToString().TrimStart();
    }
}