using CodeArchitects.Platform.Data.MongoDB.Model;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.MongoDB.Query;

internal interface IPredicateProvider
{
  Expression<Func<TEntity, bool>> GetPredicate<TEntity, TKey>(TKey key, IEntityModel entityModel)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Expression<Func<TEntity, bool>> GetPredicate<TEntity>(TEntity entity, IEntityModel entityModel)
    where TEntity : class;
}
