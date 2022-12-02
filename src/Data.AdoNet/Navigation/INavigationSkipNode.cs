using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigationSkipNode : INavigationNode
{
  new ISkipAccessibleNavigationModel Model { get; }
}
