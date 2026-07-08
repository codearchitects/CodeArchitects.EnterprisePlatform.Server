namespace CodeArchitects.Platform.Messaging.Bindings;

/// <summary>
/// Base interface for type-filtered output metadata.
/// </summary>
public interface ITypedOutputMetadata : IOutputMetadata
{
  /// <summary>
  /// The list of result types that will trigger the output binding.
  /// </summary>
  IEnumerable<Type>? AllowedTypes { get; }
}
