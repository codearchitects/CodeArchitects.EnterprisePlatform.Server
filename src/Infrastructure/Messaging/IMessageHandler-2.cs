namespace CodeArchitects.Platform.Infrastructure.Messaging;

/// <summary>
/// A class that handles messagees of type <typeparamref name="TMessage"/> and produces a result.
/// </summary>
/// <typeparam name="TMessage">The type of the handled message.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IMessageHandler<TMessage, TResult>
  where TMessage : class
  where TResult : class
{
  /// <summary>
  /// Action that is execuded in response to a message of type <typeparamref name="TMessage"/>.
  /// </summary>
  /// <param name="message">The message instance.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>The result.</returns>
  Task<TResult> HandleAsync(TMessage message, CancellationToken cancellationToken);
}
