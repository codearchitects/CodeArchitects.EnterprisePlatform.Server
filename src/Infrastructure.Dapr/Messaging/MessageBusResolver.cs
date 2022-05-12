using CodeArchitects.Platform.Common.Ioc;
using CodeArchitects.Platform.Infrastructure.Messaging;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging;

internal class MessageBusResolver : IServiceResolver<IMessageBus>
{
  private readonly DaprClient _dapr;
  private readonly DaprMessagingOptions _options;
  private readonly ILogger<MessageBusResolver>? _logger;
  private readonly ConcurrentDictionary<string, MessageBus> _messageBusses;

  /// <summary>
  /// Constructs a <see cref="MessageBusResolver"/> instance.
  /// </summary>
  /// <param name="dapr">The Dapr client.</param>
  /// <param name="configuration">The Dapr configuration.</param>
  public MessageBusResolver(DaprClient dapr, DaprMessagingOptions options, ILogger<MessageBusResolver>? logger)
  {
    _dapr = dapr;
    _options = options;
    _logger = logger;
    _messageBusses = new ConcurrentDictionary<string, MessageBus>();
  }

  public IMessageBus Resolve(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));

    if (!_options.BusNames.Contains(name))
    {
      _logger?.LogWarning("Unknown message bus requested: '{0}'", name);
    }

    return _messageBusses.GetOrAdd(name, CreateMessageBus);
  }

  private MessageBus CreateMessageBus(string busName)
  {
    return new MessageBus(_dapr, busName);
  }
}
