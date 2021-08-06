using CodeArchitects.Platform.Infrastructure.Messaging;
using Dapr.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging
{
  /// <summary>
  /// Message bus implementation using Dapr.
  /// </summary>
  public class DaprMessageBus : IMessageBus<DaprMetadata>
  {
    private readonly DaprClient _dapr;
    private readonly string _busName;

    /// <summary>
    /// Creates a new <see cref="DaprMessageBus"/> instance.
    /// </summary>
    /// <param name="dapr">The Dapr client object.</param>
    /// <param name="busName">The name of the bus.</param>
    public DaprMessageBus(DaprClient dapr, string busName)
    {
      if (dapr is null) throw new ArgumentNullException(nameof(dapr));
      if (string.IsNullOrWhiteSpace(busName)) throw new ArgumentException($"'{nameof(busName)}' cannot be null or whitespace.", nameof(busName));

      _dapr = dapr;
      _busName = busName;
    }

    public Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
      where TMessage : class
    {
      return _dapr.PublishEventAsync(_busName, DaprTopic.Make(null, typeof(TMessage)), message, cancellationToken);
    }

    public Task SendAsync<TMessage>(TMessage message, DaprMetadata metadata, CancellationToken cancellationToken = default)
      where TMessage : class
    {
      // TODO: Use metadata.AsDictionary() when Dapr will support IReadOnlyDictionary as metadata type.
      return _dapr.PublishEventAsync(_busName, DaprTopic.Make(metadata.Topic, typeof(TMessage)), message, metadata.MetadataDictionary, cancellationToken);
    }
  }
}
