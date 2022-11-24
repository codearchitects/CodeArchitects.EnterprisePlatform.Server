namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigationNode : INavigation
{
  IReadOnlyList<INavigation> Children { get; }
}
