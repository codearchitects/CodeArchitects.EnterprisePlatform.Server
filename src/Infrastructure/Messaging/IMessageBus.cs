using System.Threading;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Infrastructure.Messaging;

/// <summary>
/// Interface for sending messages to a queue.
/// </summary>
public interface IMessageBus
{
  /// <summary>
  /// Sends a message to the message queue.
  /// </summary>
  /// <typeparam name="TMessage">The type of the message.</typeparam>
  /// <param name="message">The message instance.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>A task that completes when the message is sent.</returns>
  Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class;
}