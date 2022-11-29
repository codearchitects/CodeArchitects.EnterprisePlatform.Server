using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigationSimpleNode : INavigationNode
{
  new ISimpleNavigationModel Model { get; }
}
