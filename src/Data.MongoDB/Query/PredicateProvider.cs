using CodeArchitects.Platform.Data.MongoDB.Model;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.MongoDB.Query;

internal class PredicateProvider : IPredicateProvider
{
  private readonly IPredicateTemplateFactory _templateFactory;
  private readonly IPredicateTemplateCache _cache;

  public PredicateProvider(IPredicateTemplateFactory templateFactory, IPredicateTemplateCache cache)
  {
    _templateFactory = templateFactory;
    _cache = cache;
  }

  public Expression<Func<TEntity, bool>> GetPredicate<TEntity, TKey>(TKey key, IEntityModel entityModel)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    LambdaExpression template = GetOrBuildTemplate<TEntity, TKey>(entityModel);

    return (Expression<Func<TEntity, bool>>)ValueReplacer.Replace(template, key);
  }

  public Expression<Func<TEntity, bool>> GetPredicate<TEntity>(TEntity entity, IEntityModel entityModel)
    where TEntity : class
  {
    LambdaExpression template = GetOrBuildTemplate<TEntity>(entityModel);

    return (Expression<Func<TEntity, bool>>)ValueReplacer.Replace(template, entity);
  }

  private LambdaExpression GetOrBuildTemplate<TEntity, TKey>(IEntityModel entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (!_cache.TryGetTemplate(typeof(TKey), out LambdaExpression? template))
    {
      template = _templateFactory.BuildPredicateTemplate<TEntity, TKey>(entity);
      _cache.AddTemplate(typeof(TKey), template);
    }

    return template;
  }

  private LambdaExpression GetOrBuildTemplate<TEntity>(IEntityModel entity)
    where TEntity : class
  {
    if (!_cache.TryGetTemplate(typeof(TEntity), out LambdaExpression? template))
    {
      template = _templateFactory.BuildPredicateTemplate<TEntity>(entity);
      _cache.AddTemplate(typeof(TEntity), template);
    }

    return template;
  }
}
