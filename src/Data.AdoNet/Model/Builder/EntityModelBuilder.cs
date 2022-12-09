using CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal abstract class EntityModelBuilder : BuilderBase
{
  public EntityModelBuilder(string entityName)
  {
    EntityName = entityName;
  }

  public abstract Type EntityType { get; }

  public string EntityName { get; }

  public abstract EntityModel Build();
}
