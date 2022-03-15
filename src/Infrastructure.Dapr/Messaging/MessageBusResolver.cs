using CodeArchitects.Platform.Common.Ioc;
using CodeArchitects.Platform.Infrastructure.Messaging;
using Dapr.Client;
using System;
using System.Collections.Concurrent;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging;

internal class MessageBusResolver : IServiceResolver<IMessageBus>
{
  private readonly DaprClient _dapr;
  private readonly ConcurrentDictionary<string, MessageBus> _messageBusses;

  /// <summary>
  /// Constructs a <see cref="DaprMessageBusResolver"/> instance.
  /// </summary>
  /// <param name="dapr">The Dapr client.</param>
  public MessageBusResolver(DaprClient dapr)
  {
    _dapr = dapr;
    _messageBusses = new ConcurrentDictionary<string, MessageBus>();
  }

  public IMessageBus Resolve(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));

    // TODO: Add busName validation
    return _messageBusses.GetOrAdd(name, busName => new MessageBus(_dapr, busName));
  }
}
