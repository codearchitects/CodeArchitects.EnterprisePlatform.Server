using CodeArchitects.Platform.Data.MongoDB.Model;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.MongoDB.Query;

internal interface IPredicateTemplateProvider
{
  LambdaExpression GetFindPredicateTemplate<TEntity>(IEntityModel entity)
    where TEntity : class;

  LambdaExpression GetFindPredicateTemplate<TEntity, TKey>(IEntityModel entity)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
