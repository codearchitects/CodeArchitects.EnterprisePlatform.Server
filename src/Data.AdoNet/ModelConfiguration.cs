using CodeArchitects.Platform.Data.AdoNet.Model.Builder;

namespace CodeArchitects.Platform.Data.AdoNet;

public abstract class ModelConfiguration
{
  protected abstract void Configure();

  protected void Entity<TEntity>(Action<IEntityModelBuilder<TEntity>> buildAction)
    where TEntity : class
  {

  }

  protected void Aggregation<TFrom, TTo>(Action<IAssociationBuilder<TFrom, TTo>> buildAction)
    where TFrom : class
    where TTo : class
  {

  }

  protected void Composition<TFrom, TTo>(Action<IAssociationBuilder<TFrom, TTo>> buildAction)
    where TFrom : class
    where TTo : class
  {

  }
}
