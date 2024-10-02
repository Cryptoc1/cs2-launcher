using Microsoft.AspNetCore.Components;

namespace CS2Launcher.AspNetCore.App.Infrastructure;

internal sealed class MetricsSignaler( NavigationManager navigation ) : Signaler( navigation, "/api/signals/metrics" );