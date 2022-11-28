namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class IdentityCollectionFactory : IIdentityCollectionFactory
{
  private readonly IReadOnlyDictionary<Type, object> _comparers;

  public IdentityCollectionFactory(IReadOnlyDictionary<Type, object> comparers)
  {
    _comparers = comparers;
  }

  public IdentityHashSet<TEntity> CreateHashSet<TEntity>()
    where TEntity : class
  {
    return new IdentityHashSet<TEntity>((IEqualityComparer<TEntity>)_comparers[typeof(TEntity)]);
  }

  public IdentityList<TEntity> CreateList<TEntity>()
    where TEntity : class
  {
    return new IdentityList<TEntity>((IEqualityComparer<TEntity>)_comparers[typeof(TEntity)]);
  }

  public static IdentityCollectionFactory Create(IEnumerable<EntityEmitResult> results)
  {
    IReadOnlyDictionary<Type, object> comparers = results.ToDictionary(
      keySelector: result => result.EntityType,
      elementSelector: result => Activator.CreateInstance(result.ComparerType));

    return new(comparers);
  }
}
