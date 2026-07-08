using Dapr;
using Dapr.Client;

namespace CodeArchitects.Platform.Messaging.Dapr;

internal class MessageBus : IMessageBus
{
  public const string DefaultTopic = "__global";

  private readonly DaprClient _dapr;
  private readonly IMessagingInfo _info;
  private readonly string _name;

  /// <summary>
  /// Creates a new instance of <see cref="MessageBus"/>.
  /// </summary>
  /// <param name="dapr">The Dapr client.</param>
  /// <param name="info">Info about messages.</param>
  /// <param name="name">The name of the bus.</param>
  public MessageBus(DaprClient dapr, IMessagingInfo info, string name)
  {
    _dapr = dapr;
    _info = info;
    _name = name;
  }

  public Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
  {
    if (message is null)
      throw new ArgumentNullException(nameof(message));

    return SendCoreAsync(DefaultTopic, message, cancellationToken);
  }

  public Task SendAsync<TMessage>(string topic, TMessage message, CancellationToken cancellationToken = default)
  {
    if (topic is null)
      throw new ArgumentNullException(nameof(topic));
    if (message is null)
      throw new ArgumentNullException(nameof(message));

    return SendCoreAsync(topic, message, cancellationToken);
  }

  private Task SendCoreAsync<TMessage>(string topic, TMessage message, CancellationToken cancellationToken = default)
  {
    CloudEvent<TMessage> @event = new(message)
    {
      Type = _info.GetMessageName(typeof(TMessage))
    };

    return _dapr.PublishEventAsync(_name, topic, @event, cancellationToken);
  }
}
