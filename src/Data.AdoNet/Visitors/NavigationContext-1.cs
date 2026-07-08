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
public readonly record struct NavigationContext<TNavigationModel>(object Parent, TNavigationModel NavigationModel)
  where TNavigationModel : INavigationModel
{
  public IEntityModel EntityModel => NavigationModel.To;

  internal NavigationContext WithRemovedParent() => ((NavigationContext)this).WithRemovedParent();

  public static implicit operator NavigationContext(NavigationContext<TNavigationModel> context) => new(context.Parent, context.NavigationModel);
}
