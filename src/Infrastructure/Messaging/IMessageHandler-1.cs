using System.Threading;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Infrastructure.Messaging;

/// <summary>
/// A class that handles messagees of type <typeparamref name="TMessage"/> and produces no result.
/// </summary>
/// <typeparam name="TMessage">The type of the handled message.</typeparam>
public interface IMessageHandler<TMessage>
  where TMessage : class
{
  /// <summary>
  /// Action that is execuded in response to a message of type <typeparamref name="TMessage"/>.
  /// </summary>
  /// <param name="message">The message instance.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>A task that completes when the handling finishes.</returns>
  Task HandleAsync(TMessage message, CancellationToken cancellationToken);
}