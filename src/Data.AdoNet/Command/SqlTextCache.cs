extern alias CaPlatformCommon;
using CaPlatformCommon.CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using Microsoft.Extensions.Caching.Memory;

namespace CodeArchitects.Platform.Data.AdoNet.Command;

internal class SqlTextCache : ISqlTextCache
{
  private readonly Synchronizer _synchronizer;
  private readonly IMemoryCache _cache;

  public SqlTextCache(Synchronizer synchronizer, IMemoryCache cache)
  {
    _synchronizer = synchronizer;
    _cache = cache;
  }

  public string GetOrAddFindText(INavigationRoot root, SqlTextBuilder sqlBuilder, Func<INavigationRoot, SqlTextBuilder, string> queryCompiler)
  {
    if (_cache.TryGetValue(root, out string text))
      return text;

    using (_synchronizer.Sync(root))
    {
      if (_cache.TryGetValue(root, out text))
        return text;

      text = queryCompiler(root, sqlBuilder);
      using ICacheEntry entry = _cache.CreateEntry(root);
      entry.Value = text;
      entry.Size = text.Length / 4;
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

    using (_synchronizer.Sync(key))
    {
      if (_cache.TryGetValue(key, out text))
        return text;

      text = queryCompiler(entityModel, sqlBuilder);
      using ICacheEntry entry = _cache.CreateEntry(key);
      entry.Value = text;
      entry.Size = text.Length / 4;
    }

    return text;
  }

  private sealed record CacheKey(OperationType Operation, IEntityModel EntityModel);

  private enum OperationType
  {
    Insert,
    Update,
    Upsert,
    Remove
  }
}
