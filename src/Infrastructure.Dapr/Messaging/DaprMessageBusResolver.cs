using CodeArchitects.Platform.Common.Ioc;
using CodeArchitects.Platform.Infrastructure.Messaging;
using Dapr.Client;
using System.Collections.Concurrent;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging;

/// <summary>
/// Creates and stores instances of the <see cref="DaprMessageBus"/> class.
/// </summary>
public class DaprMessageBusResolver : IServiceResolver<IMessageBus>, IServiceResolver<IMessageBus<DaprMetadata>>
{
  private readonly DaprClient _dapr;
  private readonly ConcurrentDictionary<string, DaprMessageBus> _messageBusses;

  /// <summary>
  /// Constructs a <see cref="DaprMessageBusResolver"/> instance.
  /// </summary>
  /// <param name="dapr">The Dapr client.</param>
  public DaprMessageBusResolver(DaprClient dapr)
  {
    _dapr = dapr;
    _messageBusses = new ConcurrentDictionary<string, DaprMessageBus>();
  }

  IMessageBus IServiceResolver<IMessageBus>.Resolve(string name)
    => Resolve(name);

  IMessageBus<DaprMetadata> IServiceResolver<IMessageBus<DaprMetadata>>.Resolve(string name)
    => Resolve(name);

  /// <summary>
  /// Resolves an instance of <see cref="DaprMessageBus"/> by its name.
  /// </summary>
  /// <param name="busName">The name of the bus.</param>
  /// <returns>The message bus.</returns>
  public DaprMessageBus Resolve(string busName)
  {
    // TODO: Add busName validation
    return _messageBusses.GetOrAdd(busName, name => new DaprMessageBus(_dapr, name));
  }
}
