namespace CodeArchitects.Platform.Actors.CodeAnalysis;

/// <summary>
/// Suppresses the compile-time errors detected by the analyzer.
/// </summary>
/// <remarks>
/// If design errors are detected at startup time, an <see cref="InvalidActorException"/> will be thrown.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly)]
public class DisableActorDiagnosticsAttribute : Attribute
{
}
