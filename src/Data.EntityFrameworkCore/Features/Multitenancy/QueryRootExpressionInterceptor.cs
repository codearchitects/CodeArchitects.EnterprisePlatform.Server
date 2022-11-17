using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Helpers;
using CodeArchitects.Platform.Data.Features.Multitenancy;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

internal class QueryRootExpressionInterceptor : IQueryRootExpressionInterceptor
{
  private readonly IMultitenancyContext _context;

  public QueryRootExpressionInterceptor(IMultitenancyContext context)
  {
    _context = context;
  }

  public bool ShouldApply => _context.ShouldFilter;

  public Expression Apply(Expression expression, IEntityType entity)
  {
    if (!entity.TryGetMultitenancyPredicateTemplate(out LambdaExpression? template))
      return expression;

    Expression predicate = ValueReplacer.Replace(template, _context.TenantId);

    return ExpressionHelpers.GetWhereExpression(expression, predicate, entity.ClrType);
  }
}
