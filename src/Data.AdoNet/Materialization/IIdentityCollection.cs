namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal interface IIdentityCollection
{
  void AddEntity(object entity);
  void Populate();
}
