using System.Diagnostics.CodeAnalysis;

namespace CS2Launcher.AspNetCore.App.Hosting;

/// <summary> Represents metadata about the root component. </summary>
/// <param name="ComponentType"> The type of <see cref="RootComponent"/>. </param>
public sealed record RootComponentDescriptor( [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.All )] Type ComponentType );