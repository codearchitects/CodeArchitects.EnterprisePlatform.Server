using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface ISimpleNavigationNode : INavigationNode
{
  new IAccessibleSimpleNavigationModel Model { get; }
}
