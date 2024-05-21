using CodeArchitects.Platform.Data.EntityFrameworkCore.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Query;

internal class PredicateProvider : IPredicateProvider
{
  private readonly IPredicateTemplateProvider _templateProvider;

  public PredicateProvider(IPredicateTemplateProvider templateProvider)
  {
    _templateProvider = templateProvider;
  }

  public Expression<Func<TEntity, bool>> GetFindPredicate<TEntity, TKey>(IModel model, TKey key)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    LambdaExpression template = _templateProvider.GetFindPredicateTemplate<TEntity, TKey>(model);

    Expression predicate = key is ITuple tuple
      ? TupleReplacer.Replace(template, tuple)
      : ValueReplacer.Replace(template, key);

    return (Expression<Func<TEntity, bool>>)predicate;
  }

  public Expression<Func<TEntity, bool>> GetFindPredicate<TEntity, TKey>(IModel model, TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    LambdaExpression template = _templateProvider.GetFindPredicateTemplate<TEntity, TKey>(model);

    return (Expression<Func<TEntity, bool>>)EntityReplacer.Replace(template, entity, model.FindEntityType(typeof(TEntity))!);
  }

  private sealed class EntityReplacer : ExpressionVisitor
  {
    private readonly object _entity;
    private readonly IReadOnlyList<IProperty> _keyProperties;
    private int _index;

    public EntityReplacer(object entity, IReadOnlyList<IProperty> keyProperties)
    {
      _entity = entity;
      _keyProperties = keyProperties;
    }

    protected override Expression VisitLambda<T>(Expression<T> node)
    {
      Expression result = Expression.Lambda<T>(Visit(node.Body), node.Parameters);
      _index = 0;
      return result;
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
      if (node.NodeType == ExpressionType.AndAlso)
        return Expression.AndAlso(Visit(node.Left), Visit(node.Right));

      if (node.NodeType == ExpressionType.Equal)
        return Expression.Equal(node.Left, Visit(node.Right));

      return node;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
      IProperty property = _keyProperties[_index++];

      object? value =
        property.PropertyInfo is { } propertyInfo ? propertyInfo.GetValue(_entity) :
        property.FieldInfo is { } fieldInfo ? fieldInfo.GetValue(_entity) :
        throw CouldNotGetKeyValue((IEntityType)property.DeclaringType);

      return Expression.Constant(value, node.Type);
    }


    public static Expression Replace(Expression template, object entity, IEntityType entityType)
    {
      IKey? primaryKey = entityType.FindPrimaryKey();
      if (primaryKey is null)
        throw CouldNotGetKeyValue(entityType);

      return new EntityReplacer(entity, primaryKey.Properties).Visit(template);
    }

    private static Exception CouldNotGetKeyValue(IEntityType entityType)
    {
      return new InvalidOperationException($"Could not get the key value for entity of type '{entityType.Name}'");
    }
  }
}
