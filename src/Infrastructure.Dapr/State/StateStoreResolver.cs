using CodeArchitects.Platform.Common.Ioc;
using CodeArchitects.Platform.Infrastructure.State;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace CodeArchitects.Platform.Infrastructure.Dapr.State;

/// <summary>
/// Creates and stores instances of the <see cref="StateStore"/> class.
/// </summary>
internal class StateStoreResolver : IServiceResolver<IStateStore>
{
  private readonly DaprClient _dapr;
  private readonly DaprStateOptions _options;
  private readonly ILogger<StateStoreResolver>? _logger;
  private readonly ConcurrentDictionary<string, StateStore> _stateStores;

  /// <summary>
  /// Constructs a <see cref="StateStoreResolver"/> instance.
  /// </summary>
  /// <param name="dapr">The Dapr client.</param>
  /// <param name="configuration">The Dapr configuration.</param>
  public StateStoreResolver(DaprClient dapr, DaprStateOptions options, ILogger<StateStoreResolver>? logger)
  {
    _dapr = dapr;
    _options = options;
    _logger = logger;
    _stateStores = new ConcurrentDictionary<string, StateStore>();
  }

  public IStateStore Resolve(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));

    if (!_options.StoreNames.Contains(name))
    {
      _logger?.LogWarning("Unknown state store requested: '{0}'", name);
    }

    return _stateStores.GetOrAdd(name, CreateStateStore);
  }

  private StateStore CreateStateStore(string storeName)
  {
    return new StateStore(_dapr, storeName);
  }
}
