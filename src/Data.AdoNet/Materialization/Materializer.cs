using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class Materializer : IMaterializer, IMaterializerHub
{
  private readonly IIdentityCollectionFactory _collectionFactory;
  private readonly IRowReaderProvider _readerProvider;
  private readonly Dictionary<IdentityCacheKey, object> _materializedEntities;
  private readonly List<IIdentityCollection> _identityCollections;

  public Materializer(IIdentityCollectionFactory collectionFactory, IRowReaderProvider readerProvider)
  {
    _collectionFactory = collectionFactory;
    _readerProvider = readerProvider;
    _materializedEntities = new(IdentityCacheKeyEqualityComparer.Instance);
    _identityCollections = new();
  }

  public async Task<TEntity?> ReadEntityAsync<TEntity, TKey>(DbDataReader reader, INavigationRoot<TEntity, TKey> root, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IRowReader rowReader = _readerProvider.GetRowReader(root.Entity);

    object? entity = null;
    while (await reader.ReadAsync(cancellationToken))
    {
      int offset = 0;
      entity = rowReader.ReadRow(reader, ref offset, this, root.Navigations);
    }

    foreach (IIdentityCollection list in _identityCollections)
    {
      list.Populate();
    }
    _identityCollections.Clear();

    return (TEntity?)entity;
  }

  public void AddMaterialized(IdentityCacheKey key, object materialized)
  {
    _materializedEntities.Add(key, materialized);
  }

  public bool TryGetExisting(IdentityCacheKey key, [NotNullWhen(true)] out object? existing)
  {
    return _materializedEntities.TryGetValue(key, out existing);
  }

  public IIdentityCollection CreateCollection(INavigationModel navigation)
  {
    IIdentityCollection collection = _collectionFactory.CreateCollection(navigation);
    _identityCollections.Add(collection);
    return collection;
  }

  public object? ReadRow(IDataReader reader, ref int offset, IEntityModel model, IReadOnlyCollection<INavigation> navigations)
  {
    IRowReader rowReader = _readerProvider.GetRowReader(model);

    return rowReader.ReadRow(reader, ref offset, this, navigations);
  }
}
