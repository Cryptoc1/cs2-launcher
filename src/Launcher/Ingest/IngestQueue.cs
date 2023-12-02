using System.Threading.Channels;

namespace CS2Launcher.AspNetCore.Launcher.Ingest;

internal sealed class IngestQueue
{
    private readonly Channel<LogEntry> queue = Channel.CreateBounded<LogEntry>(
        new BoundedChannelOptions( 1024 * 4 ) { FullMode = BoundedChannelFullMode.Wait }
    );

    public ValueTask<LogEntry> Dequeue( CancellationToken cancellation ) => queue.Reader.ReadAsync( cancellation );

    public ValueTask Enqueue( LogEntry entry, CancellationToken cancellation ) => queue.Writer.WriteAsync( entry, cancellation );
}