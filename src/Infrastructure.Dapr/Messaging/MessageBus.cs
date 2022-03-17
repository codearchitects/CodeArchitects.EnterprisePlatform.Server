using CodeArchitects.Platform.Infrastructure.Messaging;
using Dapr.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging;

internal class MessageBus : IMessageBus
{
  public const string DefaultTopic = "__global";

  private readonly DaprClient _dapr;
  private readonly string _name;

  /// <summary>
  /// Creates a new instance of <see cref="MessageBus"/>.
  /// </summary>
  /// <param name="dapr">The Dapr client.</param>
  /// <param name="name">The name of the bus.</param>
  public MessageBus(DaprClient dapr, string name)
  {
    _dapr = dapr;
    _name = name;
  }

  public Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
    where TMessage : class
  {
    if (message is null)
      throw new ArgumentException(nameof(message));

    return _dapr.PublishEventAsync(_name, DefaultTopic, MessageEnvelope.Create(message), cancellationToken);
  }

  public Task SendAsync<TMessage>(string topic, TMessage message, CancellationToken cancellationToken = default)
    where TMessage : class
  {
    if (message is null)
      throw new ArgumentException(nameof(message));

    return _dapr.PublishEventAsync(_name, topic, MessageEnvelope.Create(message), cancellationToken);
  }
}
