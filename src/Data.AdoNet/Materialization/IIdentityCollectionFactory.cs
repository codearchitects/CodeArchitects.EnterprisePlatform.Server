using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal interface IIdentityCollectionFactory
{
  IIdentityCollection CreateCollection(INavigationModel navigation);
}
