using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal abstract class Materializer<TEntity, TKey> : IMaterializer<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  protected readonly IMaterializerHub _hub;
  private readonly Dictionary<TKey, TEntity> _entities;

  protected Materializer(IMaterializerHub hub)
  {
    _hub = hub;
    _entities = new();
  }

  protected abstract int PropertyCount { get; }

  protected abstract TKey ReadKey(DbDataReader reader, int offset);

  protected abstract TEntity ReadEntity(DbDataReader reader, int offset);

  protected abstract void ReadNavigation(DbDataReader reader, ref int offset, TEntity entity, INavigation navigation);

  public TEntity? ReadEntity(DbDataReader reader, ref int offset, IReadOnlyCollection<INavigation> navigations)
  {
    if (reader.IsDBNull(offset))
      return null;

    TKey key = ReadKey(reader, offset);

    if (!_entities.TryGetValue(key, out TEntity? entity))
    {
      entity = ReadEntity(reader, offset);
      _entities.Add(key, entity);
    }

    offset += PropertyCount;

    foreach (INavigation navigation in navigations)
    {
      ReadNavigation(reader, ref offset, entity, navigation);
    }

    return entity;
  }

  // Used implicitly
  protected TNavigationEntity? MaterializeNavigation<TNavigationEntity, TNavigationKey>(DbDataReader reader, ref int offset, INavigation navigation, ref IMaterializer<TNavigationEntity, TNavigationKey>? materializer)
    where TNavigationEntity : class
    where TNavigationKey : IEquatable<TNavigationKey>
  {
    materializer ??= _hub.GetMaterializer<TNavigationEntity, TNavigationKey>(navigation.Target);

    return materializer.ReadEntity(reader, ref offset, navigation.Children);
  }
}
