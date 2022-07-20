using CodeArchitects.Platform.Common.Ioc;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace CodeArchitects.Platform.Messaging.Dapr;

/// <summary>
/// Creates and stores instances of the <see cref="MessageBus"/> class.
/// </summary>
internal class MessageBusResolver : IServiceResolver<IMessageBus>
{
  private readonly DaprClient _dapr;
  private readonly IMessagingInfo _info;
  private readonly ILogger _logger;
  private readonly ConcurrentDictionary<string, MessageBus> _messageBusses;

  /// <summary>
  /// Creates a new <see cref="MessageBusResolver"/> instance.
  /// </summary>
  /// <param name="dapr">The Dapr client.</param>
  /// <param name="info">Info about messaging.</param>
  /// <param name="logger">A logger instance.</param>
  public MessageBusResolver(DaprClient dapr, IMessagingInfo info, ILogger logger)
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
      _logger.LogWarning("Unknown message bus requested: '{0}'", name);
    }

    return _messageBusses.GetOrAdd(name, CreateMessageBus);
  }

  private MessageBus CreateMessageBus(string busName)
  {
    return new MessageBus(_dapr, _info, busName);
  }
}
