using CS2Launcher.AspNetCore.App.Abstractions.Signaling;

namespace CS2Launcher.AspNetCore.App.Abstractions;

public static class ConsoleSignals
{
    public sealed record Connected( string Host ) : Signal<Connected>;

    public sealed record ExecuteCommand( string Command, long Token ) : Signal<ExecuteCommand>;
    public sealed record ExecutedCommand( string Text, long Token ) : Signal<ExecutedCommand>;
    public sealed record ExecuteCommandFailed( string Error, long Token ) : Signal<ExecuteCommandFailed>;
}