using CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal abstract class EntityModelBuilder : BuilderBase
{
  public abstract Type EntityType { get; }

  public abstract EntityModel Build();

  public abstract void AddNavigation(INavigationModel navigation, IEnumerable<Name> foreignKeyNames);

  public abstract void AddColumns();
}
