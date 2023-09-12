using CodeArchitects.Platform.Data.MongoDB.Model;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.MongoDB.Query;

internal class PredicateProvider : IPredicateProvider
{
  private readonly IPredicateTemplateProvider _templateProvider;

  public PredicateProvider(IPredicateTemplateProvider templateProvider)
  {
    _templateProvider = templateProvider;
  }

  public Expression<Func<TEntity, bool>> GetFindPredicate<TEntity, TKey>(IEntityModel entityModel, TKey key)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    LambdaExpression template = _templateProvider.GetFindPredicateTemplate<TEntity, TKey>(entityModel);

    return (Expression<Func<TEntity, bool>>)ValueReplacer.Replace(template, key);
  }

  public Expression<Func<TEntity, bool>> GetFindPredicate<TEntity>(IEntityModel entityModel, TEntity entity)
    where TEntity : class
  {
    LambdaExpression template = _templateProvider.GetFindPredicateTemplate<TEntity>(entityModel);

    return (Expression<Func<TEntity, bool>>)ValueReplacer.Replace(template, entity);
  }
}
