using CodeArchitects.Platform.Data.MongoDB.Model;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.MongoDB.Query;

internal class PredicateTemplateProvider : IPredicateTemplateProvider
{
  private readonly ConcurrentDictionary<Type, LambdaExpression> _templates;

  public PredicateTemplateProvider()
  {
    _templates = new();
  }

  public LambdaExpression GetFindPredicateTemplate<TEntity, TKey>(IEntityModel entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return _templates.GetOrAdd(typeof(TEntity), (_, entity) =>
    {
      ParameterExpression entityParameter = Expression.Parameter(typeof(TEntity), "entity");

      return Expression.Lambda<Func<TEntity, bool>>(
        body: Expression.Equal(
          left: GetEntityModelKeyNameAccess(entityParameter, entity),
          right: Expression.Variable(typeof(TKey))),
        parameters: entityParameter);
    }, entity);
  }

  public LambdaExpression GetFindPredicateTemplate<TEntity>(IEntityModel entity)
    where TEntity : class
  {
    return _templates.GetOrAdd(typeof(TEntity), (_, entity) =>
    {
      ParameterExpression entityParameter = Expression.Parameter(typeof(TEntity), "entity");

      return Expression.Lambda<Func<TEntity, bool>>(
        body: Expression.Equal(
          left: GetEntityModelKeyNameAccess(entityParameter, entity),
          right: GetEntityModelKeyNameAccess(Expression.Variable(typeof(TEntity)), entity)),
        parameters: entityParameter);
    }, entity);
  }

  private Expression GetEntityModelKeyNameAccess(ParameterExpression parameterExpression, IEntityModel entity)
  {
    return Expression.Property(parameterExpression, entity.Key.Name);
  }
}
