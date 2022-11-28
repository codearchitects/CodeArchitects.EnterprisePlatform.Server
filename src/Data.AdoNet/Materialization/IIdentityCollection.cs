namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal interface IIdentityCollection<TEntity>
  where TEntity : class
{
  void AddEntity(TEntity? entity);
}
