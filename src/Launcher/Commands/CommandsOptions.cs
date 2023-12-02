namespace CS2Launcher.AspNetCore.Launcher.Commands;

public sealed class CommandsOptions
{
    public List<string> AdminIds { get; set; } = [];

    public char Token { get; set; } = '!';
}