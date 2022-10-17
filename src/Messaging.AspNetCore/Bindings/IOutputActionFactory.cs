namespace CodeArchitects.Platform.Messaging.AspNetCore.Bindings;

/// <summary>
/// Creates output actions.
/// </summary>
internal interface IOutputActionFactory
{
  /// <summary>
  /// Creates a new <see cref="OutputAction{TMetadata}"/> where <c>TMetadata</c> is <paramref name="metadataType"/>.
  /// </summary>
  /// <param name="metadataType">The type of the metadata.</param>
  /// <param name="metadata">The metadata instance.</param>
  /// <param name="services">The service provider.</param>
  /// <returns>The created instance.</returns>
  /// <exception cref="ArgumentException">Thrown when <paramref name="metadata"/> is not assignable to <paramref name="metadataType"/>.</exception>
  OutputAction CreateOutputAction(Type metadataType, object metadata, IServiceProvider services);
}
