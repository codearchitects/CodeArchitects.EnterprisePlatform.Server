namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal abstract class EntityModelBuilder : BuilderBase
{
  public EntityModelBuilder(string entityName)
  {
    EntityName = entityName;
  }

  public abstract IEntityModel Build(IEnumerable<Association> associationsTo);

  public abstract Type EntityType { get; }

  public string EntityName { get; }
}
