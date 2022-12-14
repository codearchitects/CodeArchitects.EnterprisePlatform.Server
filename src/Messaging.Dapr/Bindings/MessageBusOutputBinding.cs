using CodeArchitects.Platform.Messaging.Bindings;
using Dapr;
using Dapr.Client;
using Microsoft.Extensions.Logging;

namespace CodeArchitects.Platform.Messaging.Dapr.Bindings;

internal class MessageBusOutputBinding : IOutputBinding<IMessageBusOutputMetadata>
{
  private readonly DaprClient _dapr;
  private readonly IMessagingInfo _info;
  private readonly ILogger _logger;

  public MessageBusOutputBinding(DaprClient dapr, IMessagingInfo info, ILogger logger)
  {
    _dapr = dapr;
    _info = info;
    _logger = logger;
  }

  public Task ExecuteAsync<TMessage, TResult>(OutputBindingContext<IMessageBusOutputMetadata, TMessage, TResult> context, CancellationToken cancellationToken)
  {
    if (context.Result is not { } result)
      return Task.CompletedTask;

    string? bus = context.Metadata.Bus ?? _info.GetDefaultBus();
    if (bus is null)
    {
      _logger.LogWarning("An output binding for message type '{messageType}' and result type '{resultType}' is not bound to a bus and no default bus has been specified. The binding will be skipped.");
      return Task.CompletedTask;
    }

    string topic = context.Metadata.Topic ?? MessageBus.DefaultTopic;

    CloudEvent<TResult> @event = new(result)
    {
      Type = _info.GetMessageName(typeof(TResult))
    };

    return _dapr.PublishEventAsync(bus, topic, @event, cancellationToken);
  }
}
