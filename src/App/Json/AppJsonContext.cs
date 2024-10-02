using System.Text.Json.Serialization;

using Api = CS2Launcher.AspNetCore.App.Abstractions.Api;
using Signals = CS2Launcher.AspNetCore.App.Abstractions.Signals;

namespace CS2Launcher.AspNetCore.App.Json;

/// <summary> A <see cref="JsonSerializerContext"/> for App related models. </summary>
[JsonSerializable( typeof( Api.ApiProblem ) )]
[JsonSerializable( typeof( Api.ChangeMapParameters ) )]
[JsonSerializable( typeof( Api.ServerMemoryMetrics ) )]
[JsonSerializable( typeof( Api.ServerMetrics ) )]
[JsonSerializable( typeof( Api.ServerProcessorMetrics ) )]
[JsonSerializable( typeof( Api.ServerStatus ) )]
[JsonSerializable( typeof( bool ) )]
[JsonSerializable( typeof( Signals.ConsoleSignals.Connected ) )]
[JsonSerializable( typeof( Signals.ConsoleSignals.ConnectFailed ) )]
[JsonSerializable( typeof( Signals.ConsoleSignals.ExecuteCommand ) )]
[JsonSerializable( typeof( Signals.ConsoleSignals.ExecuteCommandFailed ) )]
[JsonSerializable( typeof( Signals.ConsoleSignals.ExecutedCommand ) )]
[JsonSerializable( typeof( Signals.MetricsSignals.Report ) )]
[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    DictionaryKeyPolicy = JsonKnownNamingPolicy.CamelCase,
    NumberHandling = JsonNumberHandling.AllowReadingFromString,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    WriteIndented = false )]
public sealed partial class AppJsonContext : JsonSerializerContext;