using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Query;

internal interface IPredicateTemplateProvider
{
  LambdaExpression GetFindPredicateTemplate<TEntity, TKey>(IModel model)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
