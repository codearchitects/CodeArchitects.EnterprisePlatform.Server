namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigationNode : INavigation
{
  IReadOnlyCollection<INavigation> Children { get; }
}
