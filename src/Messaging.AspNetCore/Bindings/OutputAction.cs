namespace CodeArchitects.Platform.Messaging.AspNetCore.Bindings;

/// <summary>
/// Action that executes after the execution of a message handler.
/// </summary>
internal abstract class OutputAction
{
  /// <summary>
  /// <c>true</c> if the output action is executed only for certain types, <c>false</c> otherwise.
  /// </summary>
  public abstract bool IsTypeFiltered { get; }

  /// <summary>
  /// Tells whether the output action can be executed for the given result type.
  /// </summary>
  /// <param name="resultType">The result type.</param>
  /// <returns><c>true</c> if the action can be executed, <c>false</c> otherwise.</returns>
  public abstract bool CanExecute(Type resultType);

  /// <summary>
  /// Executes the output action.
  /// </summary>
  /// <typeparam name="TMessage">The type of the handled message.</typeparam>
  /// <typeparam name="TResult">The type of the result.</typeparam>
  /// <param name="message">The message instance.</param>
  /// <param name="result">The result</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns></returns>
  public abstract Task ExecuteAsync<TMessage, TResult>(TMessage message, TResult? result, CancellationToken cancellationToken);
}
