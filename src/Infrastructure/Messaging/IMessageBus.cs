using System.Threading;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Infrastructure.Messaging;

/// <summary>
/// Interface for sending messages.
/// </summary>
public interface IMessageBus
{
  /// <summary>
  /// Publishes the message to the default topic of the bus.
  /// </summary>
  /// <typeparam name="TMessage">The type of the message.</typeparam>
  /// <param name="message">The message instance.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>A task that completes when the message is published.</returns>
  Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class;

  /// <summary>
  /// Publishes the message to the a topic of the bus.
  /// </summary>
  /// <typeparam name="TMessage">The type of the message.</typeparam>
  /// <param name="topic">The topic to publish the message to.</param>
  /// <param name="message">The message instance.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>A task that completes when the message is published.</returns>
  Task SendAsync<TMessage>(string topic, TMessage message, CancellationToken cancellationToken = default) where TMessage : class;
}