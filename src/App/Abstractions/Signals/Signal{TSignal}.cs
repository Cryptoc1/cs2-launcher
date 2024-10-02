namespace CS2Launcher.AspNetCore.App.Abstractions.Signals;

/// <summary> Defines a signal. </summary>
/// <typeparam name="TSignal"> The type of signal being defined. </typeparam>
public abstract record Signal<TSignal> where TSignal : Signal<TSignal>;