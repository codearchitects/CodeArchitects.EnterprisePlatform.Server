using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace CodeArchitects.Platform.Data.AdoNet.Command;

internal class SqlTextCache : ISqlTextCache
{
  private readonly IMemoryCache _cache;
  private readonly ConcurrentDictionary<object, object> _locks;

  public SqlTextCache(IMemoryCache cache)
  {
    _cache = cache;
    _locks = new();
  }

  public string GetOrAddFindText(INavigationRoot root, SqlTextBuilder sqlBuilder, Func<INavigationRoot, SqlTextBuilder, string> queryCompiler)
  {
    if (_cache.TryGetValue(root, out string text))
      return text;

    object compilationLock = _locks.GetOrAdd(root, _ => new object());
    try
    {
      lock (compilationLock)
      {
        if (_cache.TryGetValue(root, out text))
          return text;

        text = queryCompiler(root, sqlBuilder);
        _cache.Set(root, text);
      }
    }
    finally
    {
      _locks.TryRemove(root, out _);
    }

    return text;
  }

  public string GetOrAddInsertText(IEntityModel entityModel, SqlTextBuilder sqlBuilder, Func<IEntityModel, SqlTextBuilder, string> queryCompiler)
  {
    return GetOrAddText(OperationType.Insert, entityModel, sqlBuilder, queryCompiler);
  }

  public string GetOrAddUpdateText(IEntityModel entityModel, SqlTextBuilder sqlBuilder, Func<IEntityModel, SqlTextBuilder, string> queryCompiler)
  {
    return GetOrAddText(OperationType.Update, entityModel, sqlBuilder, queryCompiler);
  }

  public string GetOrAddUpsertText(IEntityModel entityModel, SqlTextBuilder sqlBuilder, Func<IEntityModel, SqlTextBuilder, string> queryCompiler)
  {
    return GetOrAddText(OperationType.Upsert, entityModel, sqlBuilder, queryCompiler);
  }

  public string GetOrAddRemoveText(IEntityModel entityModel, SqlTextBuilder sqlBuilder, Func<IEntityModel, SqlTextBuilder, string> queryCompiler)
  {
    return GetOrAddText(OperationType.Remove, entityModel, sqlBuilder, queryCompiler);
  }

  private string GetOrAddText(OperationType operation, IEntityModel entityModel, SqlTextBuilder sqlBuilder, Func<IEntityModel, SqlTextBuilder, string> queryCompiler)
  {
    CacheKey key = new(operation, entityModel);

    if (_cache.TryGetValue(key, out string text))
      return text;


    object compilationLock = _locks.GetOrAdd(key, _ => new object());
    try
    {
      lock (compilationLock)
      {
        if (_cache.TryGetValue(key, out text))
          return text;

        text = queryCompiler(entityModel, sqlBuilder);
        _cache.Set(key, text);
      }
    }
    finally
    {
      _locks.TryRemove(key, out _);
    }

    return text;
  }

  private record CacheKey(OperationType Operation, IEntityModel EntityModel);

  private enum OperationType
  {
    Insert,
    Update,
    Upsert,
    Remove
  }
}
