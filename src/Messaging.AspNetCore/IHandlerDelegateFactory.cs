using CodeArchitects.Platform.Messaging.Descriptors;

namespace CodeArchitects.Platform.Messaging.AspNetCore;

/// <summary>
/// Creates handler delegates for handler identities.
/// </summary>
internal interface IHandlerDelegateFactory
{
  /// <summary>
  /// Creates a <see cref="HandlerDelegate"/> which will execute the pipeline relative to the given handler identity.
  /// </summary>
  /// <param name="identityDescriptor">The handler identity descriptor.</param>
  /// <returns>The created handler delegate.</returns>
  HandlerDelegate CreateHandlerDelegate(IHandlerIdentityDescriptor identityDescriptor);
}
