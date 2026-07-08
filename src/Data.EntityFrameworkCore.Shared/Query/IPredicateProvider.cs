using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Query;

internal interface IPredicateProvider
{
  Expression<Func<TEntity, bool>> GetFindPredicate<TEntity, TKey>(IModel model, TKey key)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Expression<Func<TEntity, bool>> GetFindPredicate<TEntity, TKey>(IModel model, TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
