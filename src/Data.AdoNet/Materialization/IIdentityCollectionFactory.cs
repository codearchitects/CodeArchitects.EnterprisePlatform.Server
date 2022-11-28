namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal interface IIdentityCollectionFactory
{
  IdentityList<T> CreateList<T>();

  IdentityHashSet<T> CreateHashSet<T>();
}
