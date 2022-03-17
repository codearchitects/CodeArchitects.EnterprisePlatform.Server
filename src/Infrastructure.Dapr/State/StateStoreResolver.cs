using CodeArchitects.Platform.Common.Ioc;
using CodeArchitects.Platform.Infrastructure.Dapr.Configuration;
using CodeArchitects.Platform.Infrastructure.State;
using Dapr.Client;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace CodeArchitects.Platform.Infrastructure.Dapr.State;

/// <summary>
/// Creates and stores instances of the <see cref="StateStore"/> class.
/// </summary>
internal class StateStoreResolver : IServiceResolver<IStateStore>
{
  private readonly DaprClient _dapr;
  private readonly DaprConfiguration _configuration;
  private readonly ConcurrentDictionary<string, StateStore> _stateStores;

  /// <summary>
  /// Constructs a <see cref="StateStoreResolver"/> instance.
  /// </summary>
  /// <param name="dapr">The Dapr client.</param>
  /// <param name="configuration">The Dapr configuration.</param>
  public StateStoreResolver(DaprClient dapr, DaprConfiguration configuration)
  {
    _dapr = dapr;
    _configuration = configuration;
    _stateStores = new ConcurrentDictionary<string, StateStore>();
  }

  public IStateStore Resolve(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));

    return _stateStores.GetOrAdd(name, CreateStateStore);
  }

  private StateStore CreateStateStore(string storeName)
  {
    if (_configuration.Application?.StateStores is { } stateStores && !stateStores.Contains(storeName))
      throw new ArgumentException($"There is no message bus named '{storeName}'.", nameof(storeName));

    return new StateStore(_dapr, storeName);
  }
}
