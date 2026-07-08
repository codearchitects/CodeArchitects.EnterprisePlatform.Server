using CodeArchitects.Platform.Data.EntityFrameworkCore.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Query;

internal class PredicateTemplateProvider : IPredicateTemplateProvider
{
  private readonly ConcurrentDictionary<Type, LambdaExpression> _templates;

  public PredicateTemplateProvider()
  {
    _templates = new();
  }

  public LambdaExpression GetFindPredicateTemplate<TEntity, TKey>(IModel model)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return _templates.GetOrAdd(typeof(TEntity), (_, model) =>
    {
      IEntityType entityType = model.FindEntityType(typeof(TEntity))!;
      IReadOnlyList<IProperty> keyModels = entityType.FindPrimaryKey()!.Properties;

      ParameterExpression entity = Expression.Parameter(typeof(TEntity), nameof(entity));

      Expression body = typeof(ITuple).IsAssignableFrom(typeof(TKey))
        ? CreateFromTuple(entity, keyModels)
        : CreateFromScalar(entity, keyModels[0]);

      return Expression.Lambda<Func<TEntity, bool>>(body, entity);
    }, model);
  }

  private static Expression CreateFromTuple(ParameterExpression entity, IReadOnlyList<IProperty> keyModels)
  {
    Expression result = GetEqualityComponentTemplate(entity, keyModels[0]);

    for (int i = 1; i < keyModels.Count; i++)
    {
      result = Expression.AndAlso(
        left: result,
        right: GetEqualityComponentTemplate(entity, keyModels[i]));
    }
    return result;
  }

  private static Expression CreateFromScalar(ParameterExpression entity, IProperty keyModel)
  {
    return GetEqualityComponentTemplate(entity, keyModel);
  }

  private static Expression GetEqualityComponentTemplate(ParameterExpression entity, IProperty propertyModel)
  {
    return Expression.Equal(
      left: ExpressionHelpers.MakePropertyAccess(entity, propertyModel),
      right: Expression.Variable(propertyModel.ClrType));
  }
}
