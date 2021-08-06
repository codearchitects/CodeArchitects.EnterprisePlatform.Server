using CodeArchitects.Platform.Common.Ioc;
using CodeArchitects.Platform.Infrastructure.State;
using Dapr.Client;
using System.Collections.Concurrent;

namespace CodeArchitects.Platform.Infrastructure.Dapr.State
{
  /// <summary>
  /// Creates and stores instances of the <see cref="DaprStateStore"/> class.
  /// </summary>
  public class DaprStateStoreResolver : IServiceResolver<IStateStore>
  {
    private readonly DaprClient _dapr;
    private readonly ConcurrentDictionary<string, DaprStateStore> _stateStores;

    /// <summary>
    /// Constructs a <see cref="DaprStateStoreResolver"/> instance.
    /// </summary>
    /// <param name="dapr">The Dapr client.</param>
    public DaprStateStoreResolver(DaprClient dapr)
    {
      _dapr = dapr;
      _stateStores = new ConcurrentDictionary<string, DaprStateStore>();
    }

    IStateStore IServiceResolver<IStateStore>.Resolve(string name)
      => Resolve(name);

    /// <summary>
    /// Resolves an instance of <see cref="DaprStateStore"/> by its name.
    /// </summary>
    /// <param name="storeName">The name of the store.</param>
    /// <returns>The state store.</returns>
    public DaprStateStore Resolve(string storeName)
    {
      // TODO: Add storeName validation
      return _stateStores.GetOrAdd(storeName, name => new DaprStateStore(_dapr, name));
    }
  }
}
