namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal interface IIdentityCollectionFactory
{
  IIdentityCollection CreateCollection(Type propertyType);
}
