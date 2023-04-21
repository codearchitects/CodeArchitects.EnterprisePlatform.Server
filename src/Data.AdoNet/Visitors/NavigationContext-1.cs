using CodeArchitects.Platform.Common.CodeAnalysis;
using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Visitors;

[Experimental]
public readonly record struct NavigationContext<TNavigationModel>(object Parent, TNavigationModel NavigationModel)
  where TNavigationModel : INavigationModel
{
  public IEntityModel EntityModel => NavigationModel.To;

  internal NavigationContext WithRemovedParent() => ((NavigationContext)this).WithRemovedParent();

  public static implicit operator NavigationContext(NavigationContext<TNavigationModel> context) => new(context.Parent, context.NavigationModel);
}
