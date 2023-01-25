using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface ISkipNavigationNode : INavigationNode
{
  new IAccessibleSkipNavigationModel Model { get; }
}
