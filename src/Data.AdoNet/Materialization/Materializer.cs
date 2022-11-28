using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class Materializer : IMaterializer, IMaterializerHub
{
  private readonly IMaterializerFactory _factory;
  private readonly IIdentityCollectionFactory _collectionFactory;
  private readonly Dictionary<IEntityModel, object> _materializers;
  private readonly List<IIdentityList> _identityLists;

  public Materializer(IMaterializerFactory factory, IIdentityCollectionFactory collectionFactory)
  {
    _factory = factory;
    _collectionFactory = collectionFactory;
    _materializers = new();
    _identityLists = new();
  }

  public IdentityList<T> CreateList<T>()
  {
    IdentityList<T> result = _collectionFactory.CreateList<T>();
    _identityLists.Add(result);
    return result;
  }

  public IdentityHashSet<T> CreateHashSet<T>()
  {
    return _collectionFactory.CreateHashSet<T>();
  }

  public IMaterializer<TEntity, TKey> GetMaterializer<TEntity, TKey>(IEntityModel target)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (!_materializers.TryGetValue(target, out object? materializer))
    {
      materializer = _factory.CreateMaterializer<TEntity, TKey>(this);
      _materializers.Add(target, materializer);
    }

    return (IMaterializer<TEntity, TKey>)materializer;
  }

  public async Task<TEntity?> ReadEntityAsync<TEntity, TKey>(DbDataReader reader, IEntityModel target, IReadOnlyCollection<INavigation> navigations, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IMaterializer<TEntity, TKey> materializer = GetMaterializer<TEntity, TKey>(target);

    TEntity? entity = null;
    while (await reader.ReadAsync(cancellationToken))
    {
      int offset = 0;
      entity = materializer.ReadEntity(reader, ref offset, navigations);
    }

    foreach (IIdentityList list in _identityLists)
    {
      list.PopulateList();
    }
    _identityLists.Clear();

    return entity;
  }
}
