using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigation : IEquatable<INavigation>
{
  IEntityModel Target { get; }
  INavigationModel Model { get; }
  IReadOnlyCollection<INavigation> Children { get; }

  TResult Accept<TVisitor, TResult>(in TVisitor visitor)
    where TVisitor : INavigationVisitor<TResult>;

  TResult Accept<TVisitor, TResult, TState>(in TVisitor visitor, in TState state)
    where TVisitor : INavigationVisitor<TResult, TState>;
}
