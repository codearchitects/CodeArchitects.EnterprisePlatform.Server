using CodeArchitects.Platform.Messaging.AspNetCore.Bindings;
using OneOf;

namespace CodeArchitects.Platform.Messaging.AspNetCore.Handlers;

/// <summary>
/// Implementation of <see cref="HandlerDelegate"/> for handlers that produce results.
/// </summary>
/// <typeparam name="THandler">The type of the handler.</typeparam>
/// <typeparam name="TMessage">The type of the message.</typeparam>
/// <typeparam name="TResult1">The first possibile type of the result.</typeparam>
/// <typeparam name="TResult2">The second possibile type of the result.</typeparam>
/// <typeparam name="TResult3">The third possibile type of the result.</typeparam>
/// <typeparam name="TResult4">The fourth possibile type of the result.</typeparam>
/// <typeparam name="TResult5">The fifth possibile type of the result.</typeparam>
internal class HandlerDelegate<THandler, TMessage, TResult1, TResult2, TResult3, TResult4, TResult5> : BaseHandlerDelegateOneOf<THandler, TMessage, OneOf<TResult1, TResult2, TResult3, TResult4, TResult5>>
  where THandler : IMessageHandler<TMessage, OneOf<TResult1, TResult2, TResult3, TResult4, TResult5>>
{
  /// <summary>
  /// Creates a new <see cref="HandlerDelegate{THandler, TMessage, TResult1, TResult2, TResult3, TResult4, TResult5}"/> instance.
  /// </summary>
  /// <param name="outputActionsArray">The pipeline's output actions.</param>
  public HandlerDelegate(IEnumerable<OutputAction>[] outputActionsArray)
    : base(outputActionsArray)
  {
  }

  protected override int OutputActionsArrayLength => 5;

  protected override Task ExecuteOutputActionsCoreAsync(IEnumerable<OutputAction> outputActions, TMessage message, OneOf<TResult1, TResult2, TResult3, TResult4, TResult5> result, CancellationToken cancellationToken)
  {
    return result.Match(
      f0: value => ExecuteOutputActionsAsync(outputActions, message, value, cancellationToken),
      f1: value => ExecuteOutputActionsAsync(outputActions, message, value, cancellationToken),
      f2: value => ExecuteOutputActionsAsync(outputActions, message, value, cancellationToken),
      f3: value => ExecuteOutputActionsAsync(outputActions, message, value, cancellationToken),
      f4: value => ExecuteOutputActionsAsync(outputActions, message, value, cancellationToken));
  }
}
