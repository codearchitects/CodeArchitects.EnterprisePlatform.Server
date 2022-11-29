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

  public IdentityList<TEntity> CreateList<TEntity>()
    where TEntity : class
  {
    IdentityList<TEntity> result = _collectionFactory.CreateList<TEntity>();
    _identityLists.Add(result);
    return result;
  }

  public IdentityHashSet<TEntity> CreateHashSet<TEntity>()
    where TEntity : class
  {
    return _collectionFactory.CreateHashSet<TEntity>();
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

  public async Task<TEntity?> ReadEntityAsync<TEntity, TKey>(DbDataReader reader, NavigationSpec spec, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    var (target, navigations) = spec;
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
