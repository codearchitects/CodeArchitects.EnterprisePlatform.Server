using CodeArchitects.Platform.Messaging.Bindings;
using Dapr;
using Dapr.Client;

namespace CodeArchitects.Platform.Messaging.Dapr.Bindings;

internal class MessageBusOutputBinding : IOutputBinding<IMessageBusOutputMetadata>
{
  private readonly DaprClient _dapr;
  private readonly IMessagingInfo _info;

  public MessageBusOutputBinding(DaprClient dapr, IMessagingInfo info)
  {
    _dapr = dapr;
    _info = info;
  }

  public Task ExecuteAsync<TMessage, TResult>(OutputBindingContext<IMessageBusOutputMetadata, TMessage, TResult> context, CancellationToken cancellationToken)
  {
    if (context.Result is not { } result)
      return Task.CompletedTask;

    CloudEvent<TResult> @event = new CloudEvent<TResult>(result)
    {
      Type = _info.GetMessageName(typeof(TResult))
    };
    return _dapr.PublishEventAsync(context.Metadata.Bus, context.Metadata.Topic ?? MessageBus.DefaultTopic, @event, cancellationToken);
  }
}
