using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Query;

internal interface IPredicateProvider
{
  Expression<Func<TEntity, bool>> GetFindPredicate<TEntity, TKey>(TKey key)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Expression<Func<TEntity, bool>> GetFindPredicate<TEntity, TKey>(TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
