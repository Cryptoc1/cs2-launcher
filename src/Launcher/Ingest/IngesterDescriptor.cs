namespace CS2Launcher.AspNetCore.Launcher.Ingest;

internal sealed class IngesterDescriptor( IngestHandler handler, Type type )
{
    public IngestHandler Handler { get; } = handler;
    public Type Type { get; } = type;
}

public delegate Task IngestHandler( object value, CancellationToken cancellation );