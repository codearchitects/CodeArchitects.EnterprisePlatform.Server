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
  /// <param name="descriptors">The descriptors of the handlers subscribed to the endpoint.</param>
  /// <returns>The created <see cref="TopicRouter"/>.</returns>
  TopicRouter CreateRouter(IEnumerable<IHandlerDescriptor> descriptors);
}
