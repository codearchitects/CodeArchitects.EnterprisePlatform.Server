using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.Navigation;

/// <summary>
/// Represents an object that can be used to include related entities in a query using lambda expressions to specify the navigation path.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IExpressionIncluder<TEntity>
  where TEntity : class
{
  /// <summary>
  /// Includes the specified navigation entity in the query.
  /// </summary>
  /// <remarks>
  /// To include a single entity, specify the path to the navigation entity: <c>entity => entity.Navigation</c>.
  /// Sub-navigations can be added, with an arbitrary depth: <c>entity => entity.Navigation.SubNavigation</c>.
  /// Multiple navigations can be included by using an anonymous object: <c>entity => new { entity.Navigation1, entity.Navigation2 }</c>.
  /// </remarks>
  /// <typeparam name="T">The type of the related entity to include.</typeparam>
  /// <param name="includeExpression">An expression representing the path to the related entity (or entities) to include.</param>
  /// <returns>The same <see cref="IExpressionIncluder{TEntity}"/> that can be used to further specify the related entities to include.</returns>
  IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression)
    where T : class;

  /// <summary>
  /// Includes the specified navigation entity in the query, and then includes sub-navigations.
  /// </summary>
  /// <remarks>
  /// To include a single entity, specify the path to the navigation entity: <c>entity => entity.Navigation</c>.
  /// Sub-navigations can be added, with an arbitrary depth: <c>entity => entity.Navigation.SubNavigation</c>.
  /// Multiple navigations can be included by using an anonymous object: <c>entity => new { entity.Navigation1, entity.Navigation2 }</c>.
  /// </remarks>
  /// <typeparam name="T">The type of the related entity to include.</typeparam>
  /// <param name="includeExpression">An expression representing the path to the related entity (or entities) to include.</param>
  /// <param name="thenInclude">Specifies which sub-navigation entities to include in the query.</param>
  /// <returns>The same <see cref="IExpressionIncluder{TEntity}"/> that can be used to further specify the related entities to include.</returns>
  IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class;

  /// <summary>
  /// Includes the specified navigation entity in the query, and then includes sub-navigations.
  /// </summary>
  /// <remarks>
  /// To include a single entity, specify the path to the navigation entity: <c>entity => entity.Navigation</c>.
  /// Sub-navigations can be added, with an arbitrary depth: <c>entity => entity.Navigation.SubNavigation</c>.
  /// Multiple navigations can be included by using an anonymous object: <c>entity => new { entity.Navigation1, entity.Navigation2 }</c>.
  /// </remarks>
  /// <typeparam name="T">The type of the related entity to include.</typeparam>
  /// <param name="includeExpression">An expression representing the path to the related entity (or entities) to include.</param>
  /// <param name="thenInclude">Specifies which sub-navigation entities to include in the query.</param>
  /// <returns>The same <see cref="IExpressionIncluder{TEntity}"/> that can be used to further specify the related entities to include.</returns>
  IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, IEnumerable<T>?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class;
}
