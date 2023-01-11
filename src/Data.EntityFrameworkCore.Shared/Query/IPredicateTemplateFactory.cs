using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Query;

internal interface IPredicateTemplateFactory
{
  Expression<Func<TEntity, bool>> CreateFindPredicateTemplate<TEntity, TKey>()
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
