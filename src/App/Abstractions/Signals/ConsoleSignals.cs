namespace CS2Launcher.AspNetCore.App.Abstractions.Signals;

/// <summary> Signals used by the 'Console' feature. </summary>
public static class ConsoleSignals
{
    /// <summary> Represents the 'Connected' signal. </summary>
    /// <param name="Host"> The host that's been connected. </param>
    public sealed record Connected( string Host ) : Signal<Connected>;

    /// <summary> Represents the 'ConnectFailed' signal. </summary>
    /// <param name="Error"> The error that produced the failure. </param>
    public sealed record ConnectFailed( string Error ) : Signal<ConnectFailed>;

    /// <summary> Represents the 'ExecuteCommand' signal. </summary>
    /// <param name="Command"> The command to be executed. </param>
    /// <param name="Token"> A unique identifier used for traking the signal. </param>
    public sealed record ExecuteCommand( string Command, long Token ) : Signal<ExecuteCommand>;

    /// <summary> Represents the 'ExecutedCommand' signal. </summary>
    /// <param name="Text"> The result of executing a command. </param>
    /// <param name="Token"> A unique identifier used for traking the signal. </param>
    public sealed record ExecutedCommand( string Text, long Token ) : Signal<ExecutedCommand>;

    /// <summary> Represents the 'ExecuteCommandFailed' signal. </summary>
    /// <param name="Error"> The result of executing a command. </param>
    /// <param name="Token"> A unique identifier used for traking the signal. </param>
    public sealed record ExecuteCommandFailed( string Error, long Token ) : Signal<ExecuteCommandFailed>;
}