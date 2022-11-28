namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class IdentityCollectionFactory : IIdentityCollectionFactory
{
  private readonly IReadOnlyDictionary<Type, object> _comparers;

  public IdentityCollectionFactory(IReadOnlyDictionary<Type, object> comparers)
  {
    _comparers = comparers;
  }

  public IdentityHashSet<T> CreateHashSet<T>()
  {
    return new IdentityHashSet<T>((IEqualityComparer<T>)_comparers[typeof(T)]);
  }

  public IdentityList<T> CreateList<T>()
  {
    return new IdentityList<T>((IEqualityComparer<T>)_comparers[typeof(T)]);
  }

  public static IdentityCollectionFactory Create(IEnumerable<EntityEmitResult> results)
  {
    IReadOnlyDictionary<Type, object> comparers = results.ToDictionary(
      keySelector: result => result.EntityType,
      elementSelector: result => Activator.CreateInstance(result.ComparerType));

    return new(comparers);
  }
}
