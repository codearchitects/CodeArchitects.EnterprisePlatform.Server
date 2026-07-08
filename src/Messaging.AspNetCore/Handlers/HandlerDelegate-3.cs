using CodeArchitects.Platform.Messaging.AspNetCore.Bindings;

namespace CodeArchitects.Platform.Messaging.AspNetCore.Handlers;

/// <summary>
/// Implementation of <see cref="HandlerDelegate"/> for handlers that produce results.
/// </summary>
/// <typeparam name="THandler">The type of the handler.</typeparam>
/// <typeparam name="TMessage">The type of the message.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
internal class HandlerDelegate<THandler, TMessage, TResult> : BaseHandlerDelegate<THandler, TMessage>
  where THandler : IMessageHandler<TMessage, TResult>
{
  private readonly IEnumerable<OutputAction> _outputActions;

  /// <summary>
  /// Creates a new <see cref="HandlerDelegate{THandler, TMessage, TResult}"/> instance.
  /// </summary>
  /// <param name="outputActions">The pipeline's output actions.</param>
  public HandlerDelegate(IEnumerable<OutputAction> outputActions)
  {
    _outputActions = outputActions;
  }

  protected override async Task HandleAsync(THandler handler, TMessage message, CancellationToken cancellationToken)
  {
    TResult result = await handler.HandleAsync(message, cancellationToken);
    await ExecuteOutputActionsAsync(_outputActions, message, result, cancellationToken);
  }
}
