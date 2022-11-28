namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal interface IIdentityCollectionFactory
{
  IdentityList<TEntity> CreateList<TEntity>()
    where TEntity : class;

  IdentityHashSet<TEntity> CreateHashSet<TEntity>()
    where TEntity : class;
}
