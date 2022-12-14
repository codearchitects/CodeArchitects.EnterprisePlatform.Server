using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

public readonly record struct NavigationContext(object Parent, INavigationModel NavigationModel)
{
  internal NavigationContext WithRemovedParent() => this with { Parent = RemovedParent.Instance };

  internal bool HasRemovedParent => ReferenceEquals(Parent, RemovedParent.Instance);
  
  private sealed class RemovedParent
  {
    private RemovedParent() { }

    public static readonly RemovedParent Instance = new RemovedParent();
  }
}
