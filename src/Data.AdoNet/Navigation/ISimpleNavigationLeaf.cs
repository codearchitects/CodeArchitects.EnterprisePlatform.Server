using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface ISimpleNavigationLeaf : INavigationLeaf
{
  new IAccessibleSimpleNavigationModel Model { get; }
}
