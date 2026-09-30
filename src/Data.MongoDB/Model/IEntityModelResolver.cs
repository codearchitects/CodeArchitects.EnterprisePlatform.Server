namespace CodeArchitects.Platform.Data.MongoDB.Model;

/// <summary>
/// Gives access to the entity models registered with the data context.
/// </summary>
internal interface IEntityModelResolver
{
  /// <summary>
  /// Returns the registered model of <typeparamref name="TEntity"/>.
  /// </summary>
  /// <exception cref="InvalidOperationException"><typeparamref name="TEntity"/> is not registered.</exception>
  IEntityModel GetEntityModel<TEntity>();
}
