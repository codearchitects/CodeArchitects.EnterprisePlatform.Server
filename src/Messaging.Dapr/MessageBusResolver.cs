using CodeArchitects.Platform.Common.Ioc;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace CodeArchitects.Platform.Messaging.Dapr;

internal class MessageBusResolver : IServiceResolver<IMessageBus>
{
  private readonly DaprClient _dapr;
  private readonly ILogger? _logger;
  private readonly IMessagingInfo _info;
  private readonly ConcurrentDictionary<string, MessageBus> _messageBusses;

  public MessageBusResolver(DaprClient dapr, ILogger? logger, IMessagingInfo info)
  {
    _dapr = dapr;
    _logger = logger;
    _info = info;
    _messageBusses = new();
  }

  public IMessageBus Resolve(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));

    if (!_info.IsBusKnown(name))
    {
      _logger?.LogWarning("Unknown message bus requested: '{0}'", name);
    }

    return _messageBusses.GetOrAdd(name, CreateMessageBus);
  }

  private MessageBus CreateMessageBus(string busName)
  {
    return new MessageBus(_dapr, _info, busName);
  }
}
