extern alias CaPlatformCommon;
using CaPlatformCommon.CodeArchitects.Platform.Common.CodeAnalysis;
using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Visitors;

/// <summary>
/// Holds the context of the navigation.
/// </summary>
/// <param name="Parent">The parent of the current entity.</param>
/// <param name="NavigationModel">The navigation model</param>
[Experimental]
public readonly record struct NavigationContext(object Parent, INavigationModel NavigationModel)
{
  internal NavigationContext WithRemovedParent() => this with { Parent = RemovedParent.Instance };

  internal bool HasRemovedParent => ReferenceEquals(Parent, RemovedParent.Instance);

  private sealed class RemovedParent
  {
    public static readonly RemovedParent Instance = new();

    private RemovedParent() { }
  }
}
