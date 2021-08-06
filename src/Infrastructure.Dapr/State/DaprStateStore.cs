using CodeArchitects.Platform.Infrastructure.State;
using Dapr.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Infrastructure.Dapr.State
{
  /// <summary>
  /// Dapr implementation of <see cref="IStateStore"/>.
  /// </summary>
  public class DaprStateStore : IStateStore
  {
    private readonly DaprClient _dapr;
    private readonly string _storeName;

    /// <summary>
    /// Creates a new <see cref="DaprStateStore"/> instance.
    /// </summary>
    /// <param name="dapr">The Dapr client object.</param>
    /// <param name="storeName">The name of the state store.</param>
    public DaprStateStore(DaprClient dapr, string storeName)
    {
      if (dapr is null) throw new ArgumentNullException(nameof(dapr));
      if (string.IsNullOrWhiteSpace(storeName)) throw new ArgumentException($"'{nameof(storeName)}' cannot be null or whitespace.", nameof(storeName));

      _dapr = dapr;
      _storeName = storeName;
    }

    public Task<TState?> GetAsync<TState>(string key, CancellationToken cancellationToken = default)
    {
      return _dapr.GetStateAsync<TState?>(_storeName, key, cancellationToken: cancellationToken);
    }

    public Task SaveAsync<TState>(string key, TState state, CancellationToken cancellationToken = default)
    {
      return _dapr.SaveStateAsync(_storeName, key, state, cancellationToken: cancellationToken);
    }

    public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
      return _dapr.DeleteStateAsync(_storeName, key, cancellationToken: cancellationToken);
    }
  }
}
