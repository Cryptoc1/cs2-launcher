using CoreRCON;

namespace CS2Launcher.AspNetCore.Launcher.Abstractions;

/// <summary> Defines a factory for a <see cref="RCONClient"/> that connects to the "Dedicated Server". </summary>
public interface IServerConsoleFactory
{
    /// <summary> Create a new client. </summary>
    /// <remarks> Callers are responsible for disposal of the created client. </remarks>
    RCONClient Create( );
}