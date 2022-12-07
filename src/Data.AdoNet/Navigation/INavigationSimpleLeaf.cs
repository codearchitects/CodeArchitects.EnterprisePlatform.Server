using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigationSimpleLeaf : INavigationLeaf
{
  new IAccessibleSimpleNavigationModel Model { get; }
}
