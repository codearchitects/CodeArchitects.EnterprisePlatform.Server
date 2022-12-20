namespace CodeArchitects.Platform.Data.Navigation;

/// <summary>
/// Represents an object that can be used to include related entities in a query using strings to specify the navigation path.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IStringIncluder<TEntity>
  where TEntity : class
{
  /// <summary>
  /// Includes the specified navigation entity in the query.
  /// </summary>
  /// <remarks>
  /// To include a single entity, specify the name of the navigation entity: <c>"Navigation"</c>.
  /// Sub-navigations can be added, with an arbitrary depth, using the dot '.' to navigate throgh the object's graph: <c>"Navigation.SubNavigation"</c>.
  /// </remarks>
  /// <param name="navigation">A string representing the path to the related entity to include.</param>
  /// <returns>An <see cref="IStringIncluder{TEntity}"/> that can be used to further specify the related entities to include.</returns>
  IStringIncluder<TEntity> Include(string navigation);
}
