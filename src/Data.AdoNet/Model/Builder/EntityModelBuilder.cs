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

  public abstract void Complete(IReadOnlyCollection<Type> entityTypes, IEnumerable<Association> associations);

  // public abstract EntityModel EntityModel { get; }
  // 
  // public abstract PrimaryKeyModel PrimaryKeyModel { get; }
}
