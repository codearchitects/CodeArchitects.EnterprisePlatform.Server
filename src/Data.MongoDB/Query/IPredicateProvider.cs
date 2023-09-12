using CodeArchitects.Platform.Data.MongoDB.Model;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.MongoDB.Query;

internal interface IPredicateProvider
{
  Expression<Func<TEntity, bool>> GetFindPredicate<TEntity, TKey>(IEntityModel entityModel, TKey key)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Expression<Func<TEntity, bool>> GetFindPredicate<TEntity>(IEntityModel entityModel, TEntity entity)
    where TEntity : class;
}
