#pragma warning disable EF1001 // Internal EF Core API usage. Looks like we are forced to use IDbContextServices for resolving the IModel instance.

using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using System.Diagnostics;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

internal class ExpressionRewriter : ExpressionVisitor, IExpressionRewriter
{
  // Cannot use the IModel instance because it creates an infinite loop inside EF Core,
  // nor the DbContext because it can't be resolved inside its own service provider.
  private readonly IDbContextServices _services;
  private readonly IEnumerable<IQueryRootExpressionInterceptor> _interceptors;
  private IInterceptorInfo? _interceptorInfo;

  public ExpressionRewriter(IDbContextServices services, IEnumerable<IQueryRootExpressionInterceptor> interceptors)
  {
    _services = services;
    _interceptors = interceptors;
  }

  public bool ShouldRewrite(IInterceptorInfo interceptorInfo)
  {
    foreach (IQueryRootExpressionInterceptor interceptor in _interceptors)
    {
      if (interceptorInfo.IsEnabled(interceptor))
        return true;
    }
    return false;
  }

  public Expression Rewrite(Expression expression, IInterceptorInfo interceptorInfo)
  {
    _interceptorInfo = interceptorInfo;
    expression = Visit(expression);
    _interceptorInfo = null;
    return expression;
  }

  protected override Expression VisitMethodCall(MethodCallExpression node)
  {
    if (node.Arguments.Count == 0)
      return node;

    Expression queryable = Visit(node.Arguments[0]);
    if (ReferenceEquals(queryable, node.Arguments[0]))
      return node;

    Expression[] arguments = new Expression[node.Arguments.Count];
    arguments[0] = queryable;
    for (int i = 1; i < arguments.Length; i++)
    {
      arguments[i] = node.Arguments[i];
    }

    return Expression.Call(
      instance: node.Object,
      method: node.Method,
      arguments: arguments);
  }

  protected override Expression VisitExtension(Expression node)
  {
    if (node is not EntityQueryRootExpression queryRoot)
      return node;

    Type entityType = queryRoot.ElementType;
    if (_services.Model.FindEntityType(entityType) is not { } entityModel)
      return node;

    foreach (IQueryRootExpressionInterceptor interceptor in _interceptors)
    {
      if (!_interceptorInfo!.IsEnabled(interceptor))
        continue;

      node = interceptor.Apply(node, entityModel);

      EnsureCorrectExpressionType(node, entityType);
    }

    return node;
  }

  [Conditional("DEBUG")]
  private static void EnsureCorrectExpressionType(Expression node, Type entityType)
  {
    if (!node.Type.IsAssignableTo(typeof(IQueryable<>).MakeGenericType(entityType)))
      throw new InvalidOperationException("A query root expression interceptor should return an expression assignable to IQueryable of the same entity type as the original queryable.");
  }
}
