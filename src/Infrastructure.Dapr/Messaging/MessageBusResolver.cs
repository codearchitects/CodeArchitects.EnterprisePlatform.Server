using CodeArchitects.Platform.Common.Ioc;
using CodeArchitects.Platform.Infrastructure.Dapr.Configuration;
using CodeArchitects.Platform.Infrastructure.Messaging;
using Dapr.Client;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging;

internal class MessageBusResolver : IServiceResolver<IMessageBus>
{
  private readonly DaprClient _dapr;
  private readonly DaprConfiguration _configuration;
  private readonly ConcurrentDictionary<string, MessageBus> _messageBusses;

  /// <summary>
  /// Constructs a <see cref="MessageBusResolver"/> instance.
  /// </summary>
  /// <param name="dapr">The Dapr client.</param>
  /// <param name="configuration">The Dapr configuration.</param>
  public MessageBusResolver(DaprClient dapr, DaprConfiguration configuration)
  {
    _dapr = dapr;
    _configuration = configuration;
    _messageBusses = new ConcurrentDictionary<string, MessageBus>();
  }

  public IMessageBus Resolve(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));

    return _messageBusses.GetOrAdd(name, CreateMessageBus);
  }

  private MessageBus CreateMessageBus(string busName)
  {
    if (_configuration.Application?.MessageBusses is { } messageBusses && !messageBusses.Contains(busName))
      throw new ArgumentException($"There is no message bus named '{busName}'.", nameof(busName));

    return new MessageBus(_dapr, busName);
  }
}
