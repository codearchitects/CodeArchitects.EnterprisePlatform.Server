using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class Materializer : IMaterializer, IMaterializerHub
{
  private readonly IIdentityCollectionFactory _collectionFactory;
  private readonly IRowReaderProvider _readerFactory;
  private readonly Dictionary<IdentityCacheKey, object> _materializedEntities;
  private readonly List<IIdentityList> _identityLists;

  public Materializer(IIdentityCollectionFactory collectionFactory, IRowReaderProvider readerFactory)
  {
    _collectionFactory = collectionFactory;
    _readerFactory = readerFactory;
    _materializedEntities = new();
    _identityLists = new();
  }

  public async Task<TEntity?> ReadEntityAsync<TEntity, TKey>(DbDataReader reader, NavigationSpec<TEntity, TKey> spec, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IRowReader rowReader = _readerFactory.GetRowReader(spec.Entity);

    object? entity = null;
    while (await reader.ReadAsync(cancellationToken))
    {
      int offset = 0;
      entity = rowReader.ReadRow(reader, ref offset, this, spec.Navigations);
    }

    foreach (IIdentityList list in _identityLists)
    {
      list.PopulateList();
    }
    _identityLists.Clear();

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

  public IIdentityCollection CreateCollection(Type propertyType)
  {
    return _collectionFactory.CreateCollection(propertyType);
  }

  public object? ReadRow(IDataReader reader, ref int offset, IEntityModel model, IReadOnlyCollection<INavigation> navigations)
  {
    IRowReader rowReader = _readerFactory.GetRowReader(model);

    return rowReader.ReadRow(reader, ref offset, this, navigations);
  }
}
