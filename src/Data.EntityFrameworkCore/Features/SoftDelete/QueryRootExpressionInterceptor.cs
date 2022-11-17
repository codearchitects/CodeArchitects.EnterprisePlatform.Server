using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Helpers;
using CodeArchitects.Platform.Data.Features.SoftDelete;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.SoftDelete;

internal class QueryRootExpressionInterceptor : IQueryRootExpressionInterceptor
{
  private readonly ISoftDeleteContext _context;

  public QueryRootExpressionInterceptor(ISoftDeleteContext context)
  {
    _context = context;
  }

  public bool ShouldApply => _context.ShouldFilter;

  public Expression Apply(Expression expression, IEntityType entity)
  {
    if (!entity.TryGetSoftDeletePredicate(out LambdaExpression? predicate))
      return expression;

    return ExpressionHelpers.GetWhereExpression(expression, predicate, entity.ClrType);
  }
}