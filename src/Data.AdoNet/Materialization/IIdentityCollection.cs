namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal interface IIdentityCollection<T>
{
  void AddEntity(T entity);
}
