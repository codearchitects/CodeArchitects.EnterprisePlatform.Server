using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Command;

internal class SqlTextCache : ISqlTextCache
{
  private readonly IMemoryCache _cache;

  public SqlTextCache(IMemoryCache cache)
  {
    _cache = cache;
  }

  public bool TryGetFindText(INavigationRoot root, [NotNullWhen(true)] out string? text)
  {
    if (!_cache.TryGetValue(root, out object? value))
    {
      text = null;
      return false;
    }

    text = (string)value;
    return true;
  }

  public void AddFindText(INavigationRoot root, string text)
  {
    ICacheEntry entry = _cache.CreateEntry(root);
    entry.Value = text;
  }

  public bool TryGetInsertText(IEntityModel entityModel, [NotNullWhen(true)] out string? text)
  {
    return TryGetText(OperationType.Insert, entityModel, out text);
  }

  public void AddInsertText(IEntityModel entityModel, string text)
  {
    AddText(OperationType.Insert, entityModel, text);
  }

  public bool TryGetUpdateText(IEntityModel entityModel, [NotNullWhen(true)] out string? text)
  {
    return TryGetText(OperationType.Update, entityModel, out text);
  }

  public void AddUpdateText(IEntityModel entityModel, string text)
  {
    AddText(OperationType.Update, entityModel, text);
  }

  public bool TryGetUpsertText(IEntityModel entityModel, [NotNullWhen(true)] out string? text)
  {
    return TryGetText(OperationType.Upsert, entityModel, out text);
  }

  public void AddUpsertText(IEntityModel entityModel, string text)
  {
    AddText(OperationType.Upsert, entityModel, text);
  }

  public bool TryGetRemoveText(IEntityModel entityModel, [NotNullWhen(true)] out string? text)
  {
    return TryGetText(OperationType.Remove, entityModel, out text);
  }

  public void AddRemoveText(IEntityModel entityModel, string text)
  {
    AddText(OperationType.Remove, entityModel, text);
  }

  private bool TryGetText(OperationType operation, IEntityModel entityModel, [NotNullWhen(true)] out string? text)
  {
    CacheKey key = new(operation, entityModel);
    if (!_cache.TryGetValue(key, out object? value))
    {
      text = null;
      return false;
    }

    text = (string)value;
    return true;
  }

  private void AddText(OperationType operation, IEntityModel entityModel, string text)
  {
    CacheKey key = new(operation, entityModel);
    ICacheEntry entry = _cache.CreateEntry(key);
    entry.Value = text;
  }

  private record CacheKey(OperationType Operation, IEntityModel EntityModel);

  private enum OperationType
  {
    Find,
    Insert,
    Update,
    Upsert,
    Remove
  }
}
