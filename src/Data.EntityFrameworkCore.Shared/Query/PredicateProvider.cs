using CodeArchitects.Platform.Data.EntityFrameworkCore.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Query;

internal class PredicateProvider : IPredicateProvider
{
  private readonly IPredicateTemplateFactory _templateFactory;
  private readonly IPredicateTemplateCache _cache;
  private readonly IModel _model; // TODO: Make DataContext pass the IEntityModel to each method call

  public PredicateProvider(IPredicateTemplateFactory templateFactory, IPredicateTemplateCache cache, IModel model)
  {
    _templateFactory = templateFactory;
    _cache = cache;
    _model = model;
  }

  public Expression<Func<TEntity, bool>> GetFindPredicate<TEntity, TKey>(TKey key)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    LambdaExpression template = GetOrBuildTemplate<TEntity, TKey>();

    Expression predicate = key is ITuple tuple
      ? TupleReplacer.Replace(template, tuple)
      : ValueReplacer.Replace(template, key);

    return (Expression<Func<TEntity, bool>>)predicate;
  }

  public Expression<Func<TEntity, bool>> GetFindPredicate<TEntity, TKey>(TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    LambdaExpression template = GetOrBuildTemplate<TEntity, TKey>();

    return (Expression<Func<TEntity, bool>>)EntityReplacer.Replace(template, entity, _model.FindEntityType(typeof(TEntity))!);
  }

  private LambdaExpression GetOrBuildTemplate<TEntity, TKey>()
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (!_cache.TryGetTemplate(typeof(TEntity), out LambdaExpression? template))
    {
      template = _templateFactory.CreateFindPredicateTemplate<TEntity, TKey>();
      _cache.AddTemplate(typeof(TEntity), template);
    }

    return template;
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
        throw CouldNotGetKeyValue(property.DeclaringEntityType);

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
