using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigation : IEquatable<INavigation>
{
  int Id { get; }
  IEntityModel Target { get; }
  INavigationModel Model { get; } // TODO: Either expose all necessary properties of the model without exposing the model, or expose the model only.
  IReadOnlyCollection<INavigation> Children { get; }

  TResult Accept<TVisitor, TResult>(in TVisitor visitor)
    where TVisitor : INavigationVisitor<TResult>;

  TResult Accept<TVisitor, TResult, TState>(in TVisitor visitor, in TState state)
    where TVisitor : INavigationVisitor<TResult, TState>;
}
