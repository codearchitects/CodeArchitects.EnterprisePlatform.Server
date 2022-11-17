using CodeArchitects.Platform.Data.EntityFrameworkCore.Helpers;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Query;

internal class PredicateProvider : IPredicateProvider
{
  private readonly IPredicateTemplateFactory _templateFactory;
  private readonly IPredicateTemplateCache _cache;

  public PredicateProvider(IPredicateTemplateFactory templateFactory, IPredicateTemplateCache cache)
  {
    _templateFactory = templateFactory;
    _cache = cache;
  }

  public Expression<Func<TEntity, bool>> GetFindPredicate<TEntity, TKey>(TKey key)
  {
    LambdaExpression template = GetOrBuildTemplate<TEntity, TKey>();

    Expression predicate = key is ITuple tuple
      ? TupleReplacer.Replace(template, tuple)
      : ValueReplacer.Replace(template, key);

    return (Expression<Func<TEntity, bool>>)predicate;
  }

  private LambdaExpression GetOrBuildTemplate<TEntity, TKey>()
  {
    if (!_cache.TryGetTemplate(typeof(TEntity), out LambdaExpression? template))
    {
      template = _templateFactory.CreateFindPredicateTemplate<TEntity, TKey>();
      _cache.AddTemplate(typeof(TEntity), template);
    }

    return template;
  }
}
