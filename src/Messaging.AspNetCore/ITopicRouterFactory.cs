using CodeArchitects.Platform.Messaging.Descriptors;

namespace CodeArchitects.Platform.Messaging.AspNetCore;

/// <summary>
/// Creates topic routers for handler identities sharing bus and topic.
/// </summary>
internal interface ITopicRouterFactory
{
  /// <summary>
  /// Creates a <see cref="TopicRouter"/> which routes messages delivered to the same endpoint to the correct <see cref="HandlerDelegate"/>.
  /// </summary>
  /// <param name="identityDescriptors">A collection of handler identities which share the same bus and topic.</param>
  /// <returns>The created <see cref="TopicRouter"/>.</returns>
  TopicRouter CreateRouter(IEnumerable<IHandlerIdentityDescriptor> identityDescriptors);
}
