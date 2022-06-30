namespace CodeArchitects.Platform.Messaging.AspNetCore.Bindings;

/// <summary>
/// Action that executes after the execution of a message handler.
/// </summary>
internal abstract class OutputAction
{
  /// <summary>
  /// Executes the output action.
  /// </summary>
  /// <typeparam name="TMessage">The type of the handled message.</typeparam>
  /// <typeparam name="TResult">The type of the result.</typeparam>
  /// <param name="message">The message instance.</param>
  /// <param name="result">The result</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns></returns>
  public abstract Task ExecuteAsync<TMessage, TResult>(TMessage message, TResult? result, CancellationToken cancellationToken)
    where TMessage : class
    where TResult : class;
}
