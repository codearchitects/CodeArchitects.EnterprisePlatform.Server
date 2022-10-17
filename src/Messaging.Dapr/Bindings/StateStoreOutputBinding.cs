using CodeArchitects.Platform.Messaging.Bindings;
using Dapr.Client;

namespace CodeArchitects.Platform.Messaging.Dapr.Bindings;

internal class StateStoreOutputBinding : IOutputBinding<IStateStoreOutputMetadata>
{
  private readonly DaprClient _dapr;

  public StateStoreOutputBinding(DaprClient dapr)
  {
    _dapr = dapr;
  }

  public Task ExecuteAsync<TMessage, TResult>(OutputBindingContext<IStateStoreOutputMetadata, TMessage, TResult> context, CancellationToken cancellationToken)
  {
    if (context.Result is not { } result)
      return Task.CompletedTask;

    return _dapr.SaveStateAsync(context.Metadata.StoreName, context.Metadata.Key, result, cancellationToken: cancellationToken);
  }
}
