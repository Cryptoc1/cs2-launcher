namespace CS2Launcher.AspNetCore.App.Abstractions.Api;

/// <summary> Represents the details of a problem that occurred while invoking a Middleware API. </summary>
public sealed record class ApiProblem
{
    /// <summary> Additional description and summarization of the problem. </summary>
    public string? Detail { get; init; }

    /// <summary> A collection of keyed errors. </summary>
    public IDictionary<string, string[]> Errors { get; init; } = new Dictionary<string, string[]>();

    /// <summary> A user-friendly description of the problem. </summary>
    public string? Title { get; init; }

    /// <summary> A unique identifier of the request for use with logging+tracing. </summary>
    public string? TraceId { get; init; }

    /// <summary> A identifier of the problem that occurred. </summary>
    public string? Type { get; init; }
}