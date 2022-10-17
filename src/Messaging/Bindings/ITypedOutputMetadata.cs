namespace CodeArchitects.Platform.Messaging.Bindings;

/// <summary>
/// Base interface for type-filtered output metadata.
/// </summary>
public interface ITypedOutputMetadata : IOutputMetadata
{
  IEnumerable<Type>? AllowedTypes { get; }
}
