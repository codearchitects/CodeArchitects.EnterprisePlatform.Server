using CodeArchitects.Platform.Messaging.Descriptors;
using System.Collections.Concurrent;
using System.Reflection;

namespace CodeArchitects.Platform.Messaging.AspNetCore;

/// <summary>
/// Implementation of <see cref="ITopicRouterFactory"/>.
/// </summary>
internal class TopicRouterFactory : ITopicRouterFactory
{
  private readonly IHandlerDelegateFactory _delegateFactory;
  private readonly IReadOnlyDictionary<string, Type> _messageTypes;

  public TopicRouterFactory(
    IHandlerDelegateFactory delegateFactory,
    IReadOnlyDictionary<string, Type> messageTypes)
  {
    _delegateFactory = delegateFactory;
    _messageTypes = messageTypes;
  }

  public TopicRouter CreateRouter(IEnumerable<IHandlerIdentityDescriptor> identityDescriptors)
  {
    Dictionary<string, HandlerDelegate> delegates = new Dictionary<string, HandlerDelegate>();
    foreach (IHandlerIdentityDescriptor identityDescriptor in identityDescriptors)
    {
      Type messageType = identityDescriptor.MessageType;
      string messageName = messageType.GetCustomAttribute<MessageAttribute>()?.MessageName ?? messageType.Name;
      HandlerDelegate @delegate = _delegateFactory.CreateHandlerDelegate(identityDescriptor);

      if (!delegates.TryAdd(messageName, @delegate))
        throw new InvalidOperationException($"Duplicate message {messageName} on bus '{identityDescriptor.Bus}' and topic '{identityDescriptor.Topic}'.");
    }

    return new TopicRouter(new ConcurrentDictionary<string, HandlerDelegate>(delegates), _messageTypes);
  }
}
