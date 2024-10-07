using System.ComponentModel.DataAnnotations;
using CS2Launcher.AspNetCore.App.Annotations;

namespace CS2Launcher.AspNetCore.App.Abstractions.Api;

/// <summary> Represents the "Server API". </summary>
public interface IServerApi
{
    /// <summary> Request a map change on the server. </summary>
    /// <param name="parameters"> The request parameters. </param>
    /// <param name="cancellation"> A token that may cancel the request. </param>
    /// <returns> A value indicating whether the request was successfully submitting. </returns>
    Task<bool> ChangeMap( ChangeMapParameters parameters, CancellationToken cancellation = default );

    /// <summary> Retrieve performance metrics of the server. </summary>
    /// <param name="cancellation"> A token that may cancel the request. </param>
    Task<ServerMetrics> Metrics( CancellationToken cancellation = default );

    /// <summary> Retrieve the status of the server. </summary>
    /// <param name="cancellation"> A token that may cancel the request. </param>
    Task<ServerStatus> Status( CancellationToken cancellation = default );
}

/// <summary> Represents the parameters for changing the map of the server. </summary>
public sealed record class ChangeMapParameters
{
    /// <summary> The id or url of the workshop map to change to. </summary>
    [Required]
    [WorkshopId]
    public string WorkshopId { get; set; }
}

/// <summary> Represents performance metrics of the server. </summary>
/// <param name="Memory"> The server's memory metrics. </param>
/// <param name="Processor"> The server's processor metrics. </param>
/// <param name="Status"> The server's current state. </param>
public sealed record class ServerMetrics( ServerMemoryMetrics Memory, ServerProcessorMetrics Processor, ServerStatus Status )
{
    /// <summary> Represents metrics where each value is <c>0</c>. </summary>
    public static readonly ServerMetrics Zero = new( ServerMemoryMetrics.Zero, ServerProcessorMetrics.Zero, default );
}

/// <summary> Represents the server's memory usage metrics. </summary>
/// <param name="Paged"> The number of bytes in paged memory. </param>
/// <param name="Virtual"> The number of bytes in virtual memory. </param>
/// <param name="Working"> The number of bytes in working memory. </param>
public sealed record class ServerMemoryMetrics( long Paged, long Virtual, long Working )
{
    /// <summary> Represents metrics where each value is <c>0</c>. </summary>
    public static readonly ServerMemoryMetrics Zero = new( 0, 0, 0 )
    {
        Peak = new( 0, 0, 0 )
    };

    /// <summary> Memory metrics representing peak memory usage. </summary>
    public ServerMemoryMetrics? Peak { get; init; }
}

/// <summary> Represents the server's process usage metrics. </summary>
/// <param name="Privileged"> The privileged processor time. </param>
/// <param name="Total"> The total processor time. </param>
/// <param name="User"> The user processor time. </param>
public sealed record class ServerProcessorMetrics( TimeSpan Privileged, TimeSpan Total, TimeSpan User )
{
    /// <summary> Represents metrics where each value is <c>0</c>. </summary>
    public static readonly ServerProcessorMetrics Zero = new( TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero );
}

/// <summary> Represents the status of the server. </summary>
public enum ServerStatus : byte
{
    /// <summary> Indicates the server has not yet been started. </summary>
    NotStarted,

    /// <summary> Indicates the server is currently starting. </summary>
    Starting,

    /// <summary> Indicates the server the server is running. </summary>
    Running,

    /// <summary> Indicates the server has crashed. </summary>
    Crashed
}