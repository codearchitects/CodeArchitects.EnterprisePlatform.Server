namespace CodeArchitects.Platform.Data.Navigation;

/// <summary>
/// Represents an object that can be used to include related entities.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IIncluder<TEntity> : IExpressionIncluder<TEntity>, IStringIncluder<TEntity>
  where TEntity : class
{
}
