using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface ISkipNavigationLeaf : INavigationLeaf
{
  new IAccessibleSkipNavigationModel Model { get; }
}
