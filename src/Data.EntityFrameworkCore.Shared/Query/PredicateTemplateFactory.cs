using CodeArchitects.Platform.Data.EntityFrameworkCore.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Query;

internal class PredicateTemplateFactory : IPredicateTemplateFactory
{
  private readonly IModel _model;

  public PredicateTemplateFactory(IModel model)
  {
    _model = model;
  }

  public Expression<Func<TEntity, bool>> CreateFindPredicateTemplate<TEntity, TKey>()
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityType entityType = _model.FindEntityType(typeof(TEntity))!;
    IReadOnlyList<IProperty> keyModels = entityType.FindPrimaryKey()!.Properties;

    ParameterExpression entity = Expression.Parameter(typeof(TEntity), nameof(entity));

    Expression body = typeof(ITuple).IsAssignableFrom(typeof(TKey))
      ? CreateFromTuple<TEntity>(entity, keyModels)
      : CreateFromScalar<TEntity>(entity, keyModels[0]);

    return Expression.Lambda<Func<TEntity, bool>>(body, entity);
  }

  private static Expression CreateFromTuple<TEntity>(ParameterExpression entity, IReadOnlyList<IProperty> keyModels)
  {
    Expression result = GetEqualityComponentTemplate<TEntity>(entity, keyModels[0]);

    for (int i = 1; i < keyModels.Count; i++)
    {
      result = Expression.AndAlso(
        left: result,
        right: GetEqualityComponentTemplate<TEntity>(entity, keyModels[i]));
    }
    return result;
  }

  private static Expression CreateFromScalar<TEntity>(ParameterExpression entity, IProperty keyModel)
  {
    return GetEqualityComponentTemplate<TEntity>(entity, keyModel);
  }

  private static Expression GetEqualityComponentTemplate<TEntity>(ParameterExpression entity, IProperty propertyModel)
  {
    return Expression.Equal(
      left: ExpressionHelpers.MakePropertyAccess(entity, propertyModel),
      right: Expression.Variable(propertyModel.ClrType));
  }
}
