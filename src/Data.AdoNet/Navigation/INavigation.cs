using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigation
{
  int Index { get; }
  IEntityModel Target { get; }
  INavigationModel Model { get; }

  void Accept<TVisitor>(in TVisitor visitor)
    where TVisitor : INavigationVisitor;

  void Accept<TVisitor, TState>(in TVisitor visitor, in TState state)
    where TVisitor : INavigationVisitor<TState>;
}