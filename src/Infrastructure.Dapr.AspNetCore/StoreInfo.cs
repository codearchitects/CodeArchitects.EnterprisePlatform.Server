using CodeArchitects.Platform.Dapr.AspNetCore.Components;
using CodeArchitects.Platform.Infrastructure.Dapr.State;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore;

internal class StoreInfo : IStoreInfo
{
  private readonly HashSet<string> _storeNames;
  private readonly string? _defaultStore;

  public StoreInfo(HashSet<string> storeNames, string? defaultStore)
  {
    _storeNames = storeNames;
    _defaultStore = defaultStore;
  }

  public bool IsStoreKnown(string storeName)
  {
    if (_defaultStore is not null && _defaultStore == storeName)
      return true;

    if (_defaultStore is null)
      return true;

    return _storeNames.Contains(storeName);
  }

  public string? GetDefaultStore()
  {
    return _defaultStore is not null
      ? _defaultStore
      : _storeNames.Count is 1
        ? _storeNames.First()
        : null;
  }

  public static StoreInfo Create(IDaprComponentAccessor componentAccessor, string? defaultStore)
  {
    HashSet<string> storeNames = new();
    foreach (string storeName in componentAccessor.Components.GetComponentNames("state"))
    {
      storeNames.Add(storeName);
    }

    return new StoreInfo(storeNames, defaultStore);
  }
}
