namespace CodeArchitects.Platform.Messaging.Descriptors;

/// <summary>
/// Describes an action that will be executed after the message handler.
/// </summary>
internal interface IOutputBindingDescriptor
{
  /// <summary>
  /// The type of the metadata associated to the binding.
  /// </summary>
  Type MetadataType { get; }

  /// <summary>
  /// <see langword="true"/> if the metadata type is type-filtered, <see langword="false"/> otherwise.
  /// </summary>
  bool IsTypeFiltered { get; }

  /// <summary>
  /// The instance of the metadata associated to the binding.
  /// </summary>
  object MetadataObject { get; }
}
