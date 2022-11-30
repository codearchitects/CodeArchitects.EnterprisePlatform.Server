using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal readonly struct NavigationContext
{
  public NavigationContext(object parent, INavigationModel navigationModel)
  {
    Parent = parent;
    NavigationModel = navigationModel;
  }

  public object Parent { get; }
  public INavigationModel NavigationModel { get; }
}