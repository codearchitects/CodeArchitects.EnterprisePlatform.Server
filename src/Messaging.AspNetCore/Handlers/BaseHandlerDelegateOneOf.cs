using CodeArchitects.Platform.Messaging.AspNetCore.Bindings;
using OneOf;

namespace CodeArchitects.Platform.Messaging.AspNetCore.Handlers;

internal abstract class BaseHandlerDelegateOneOf<THandler, TMessage, TOneOf> : BaseHandlerDelegate<THandler, TMessage>
  where TOneOf : IOneOf
  where THandler : IMessageHandler<TMessage, TOneOf>
{
  private readonly IEnumerable<OutputAction>[] _outputActionsArray;

  /// <summary>
  /// Creates a new <see cref="BaseHandlerDelegateOneOf{THandler, TMessage}"/> instance.
  /// </summary>
  /// <param name="outputActionsArray">The pipeline's output actions.</param>
  protected BaseHandlerDelegateOneOf(IEnumerable<OutputAction>[] outputActionsArray)
  {
    if (outputActionsArray.Length != OutputActionsArrayLength)
      throw new ArgumentException($"Expected an array of output actions of length {OutputActionsArrayLength}, but got an array of length {outputActionsArray.Length}", nameof(outputActionsArray));

    _outputActionsArray = outputActionsArray;
  }

  protected abstract int OutputActionsArrayLength { get; }

  protected sealed override async Task HandleAsync(THandler handler, TMessage message, CancellationToken cancellationToken)
  {
    TOneOf result = await handler.HandleAsync(message, cancellationToken);
    await ExecuteOutputActionsCoreAsync(_outputActionsArray[result.Index], message, result, cancellationToken);
  }

  protected abstract Task ExecuteOutputActionsCoreAsync(IEnumerable<OutputAction> outputActions, TMessage message, TOneOf result, CancellationToken cancellationToken);
}
