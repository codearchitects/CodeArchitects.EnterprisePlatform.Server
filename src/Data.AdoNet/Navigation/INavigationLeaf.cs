using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigationLeaf : INavigation
{
  new INavigationModel Model { get; }
}
