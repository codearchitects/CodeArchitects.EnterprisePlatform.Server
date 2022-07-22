using CodeArchitects.Platform.Messaging.Bindings;

namespace CodeArchitects.Platform.Messaging.AspNetCore.Bindings;

/// <summary>
/// Implementation of <see cref="OutputAction"/> for type-filtered metadata.
/// </summary>
/// <typeparam name="TMetadata">The metadata type.</typeparam>
internal class TypedOutputAction<TMetadata> : OutputAction<TMetadata>
  where TMetadata : ITypedOutputMetadata
{
  /// <summary>
  /// Creates a new <see cref="TypedOutputAction{TMetadata}"/>.
  /// </summary>
  /// <param name="metadata">The metadata instance.</param>
  /// <param name="services">The service provider.</param>
  public TypedOutputAction(TMetadata metadata, IServiceProvider services)
    : base(metadata, services)
  {
  }

  public override bool IsTypeFiltered => true;

  public override bool CanExecute(Type resultType)
  {
    if (_metadata.AllowedTypes is null)
      return true;

    foreach (Type allowedType in _metadata.AllowedTypes)
    {
      if (resultType == allowedType)
        return true;
    }
    return false;
  }
}
