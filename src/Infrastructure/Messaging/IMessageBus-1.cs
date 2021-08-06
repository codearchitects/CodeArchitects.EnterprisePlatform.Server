using System.Threading;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Infrastructure.Messaging
{
  /// <summary>
  /// Extension to the <see cref="IMessageBus"/> interface with metadata support.
  /// </summary>
  /// <typeparam name="TMetadata">The metadata type of this bus.</typeparam>
  public interface IMessageBus<in TMetadata> : IMessageBus
    where TMetadata : class
  {
    /// <summary>
    /// Sends a message to the message queue with associated metadata.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <param name="message">The message instance.</param>
    /// <param name="metadata">Data to be supplied to the bus along with the message.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A task that completes when the message is sent.</returns>
    Task SendAsync<TMessage>(TMessage message, TMetadata metadata, CancellationToken cancellationToken = default) where TMessage : class;
  }
}
