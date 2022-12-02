using CodeArchitects.Platform.Data.AdoNet.Model.Builder;

namespace CodeArchitects.Platform.Data.AdoNet;

public abstract class ModelConfiguration
{
  protected abstract void Configure();

  protected void Entity<TEntity>(Action<EntityModelBuilder<TEntity>> buildAction)
    where TEntity : class
  {

  }
}
