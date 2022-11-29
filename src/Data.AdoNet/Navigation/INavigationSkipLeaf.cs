using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigationSkipLeaf : INavigation
{
  new ISkipNavigationModel Model { get; }
}
