using CodeArchitects.Platform.Messaging.AspNetCore.Utils;
using CodeArchitects.Platform.Messaging.Descriptors;
using System.Collections.Concurrent;

namespace CodeArchitects.Platform.Messaging.AspNetCore;

/// <summary>
/// Implementation of <see cref="ITopicRouterFactory"/>.
/// </summary>
internal class TopicRouterFactory : ITopicRouterFactory
{
  private readonly IHandlerDelegateFactory _delegateFactory;
  private readonly IMessageBiMap _messageMap;

  public TopicRouterFactory(
    IHandlerDelegateFactory delegateFactory,
    IMessageBiMap messageMap)
  {
    _delegateFactory = delegateFactory;
    _messageMap = messageMap;
  }

  public TopicRouter CreateRouter(IEnumerable<IHandlerDescriptor> descriptors)
  {
    Dictionary<string, HandlerDelegate> delegates = new Dictionary<string, HandlerDelegate>();
    foreach (IHandlerDescriptor handlerDescriptor in descriptors)
    {
      string messageName = _messageMap[handlerDescriptor.MessageType];
      HandlerDelegate @delegate = _delegateFactory.CreateHandlerDelegate(handlerDescriptor);

      if (!delegates.TryAdd(messageName, @delegate))
        throw new InvalidOperationException($"Duplicate message '{messageName}' handler on bus '{handlerDescriptor.Bus}' and topic '{handlerDescriptor.Topic}'.");
    }

    return new TopicRouter(new ConcurrentDictionary<string, HandlerDelegate>(delegates), _messageMap);
  }
}
