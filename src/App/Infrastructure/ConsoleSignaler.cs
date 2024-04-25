using Microsoft.AspNetCore.Components;

namespace CS2Launcher.AspNetCore.App.Infrastructure;

internal sealed class ConsoleSignaler( NavigationManager navigation ) : Signaler( navigation, "/api/signals/console" );