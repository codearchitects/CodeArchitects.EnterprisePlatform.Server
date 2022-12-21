using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

/// <summary>
/// Intercepts a query expression.
/// </summary>
public interface IQueryRootExpressionInterceptor
{
  /// <summary>
  /// If <c>true</c> the interceptor will execute, if <c>false</c> the interceptor will not execute.
  /// </summary>
  bool ShouldApply { get; }

  /// <summary>
  /// Applies the modification to the query expression. The supplied expression is a root entity expression or the result of previous interceptors.
  /// </summary>
  /// <param name="expression">The expression to intercept.</param>
  /// <param name="entity">The entity type.</param>
  /// <returns>The modified expression.</returns>
  Expression Apply(Expression expression, IEntityType entity);
}
