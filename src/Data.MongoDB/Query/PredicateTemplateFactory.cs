using CodeArchitects.Platform.Data.MongoDB.Model;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.MongoDB.Query;

internal class PredicateTemplateFactory : IPredicateTemplateFactory
{
  public Expression<Func<TEntity, bool>> BuildPredicateTemplate<TEntity, TKey>(IEntityModel entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    ParameterExpression entityParameter = Expression.Parameter(typeof(TEntity), "entity");

    return Expression.Lambda<Func<TEntity, bool>>(
      body: Expression.Equal(
        left: GetEntityModelKeyNameAccess(entityParameter, entity),
        right: Expression.Variable(typeof(TKey))),
      parameters: entityParameter);
  }

  public Expression<Func<TEntity, bool>> BuildPredicateTemplate<TEntity>(IEntityModel entity)
    where TEntity : class
  {
    ParameterExpression entityParameter = Expression.Parameter(typeof(TEntity), "entity");

    return Expression.Lambda<Func<TEntity, bool>>(
      body: Expression.Equal(
        left: GetEntityModelKeyNameAccess(entityParameter, entity),
        right: GetEntityModelKeyNameAccess(Expression.Variable(typeof(TEntity)), entity)),
      parameters: entityParameter);
  }

  private Expression GetEntityModelKeyNameAccess(ParameterExpression parameterExpression, IEntityModel entity)
  {
    return Expression.Property(parameterExpression, entity.Key.Name);
  }
}
