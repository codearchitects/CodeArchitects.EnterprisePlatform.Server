using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigation : IEquatable<INavigation>
{
  IEntityModel Target { get; }
  
  IAccessibleNavigationModel Model { get; }
  
  IReadOnlyCollection<INavigation> Children { get; }

  TResult Accept<TVisitor, TResult>(in TVisitor visitor)
    where TVisitor : INavigationVisitor<TResult>;
}
