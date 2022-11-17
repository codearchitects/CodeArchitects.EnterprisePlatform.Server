#pragma warning disable EF1001 // Internal EF Core API usage. Using this until a better solution is found. https://github.com/dotnet/efcore/issues/29573

using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

internal class InterceptedEntityQueryProvider : EntityQueryProvider, IInterceptedEntityQueryProvider
{
  private readonly IExpressionRewriter _rewriter;
  private Dictionary<Type, bool>? _interceptorTypes;

  public InterceptedEntityQueryProvider(IQueryCompiler queryCompiler, IExpressionRewriter rewriter)
    : base(queryCompiler)
  {
    _rewriter = rewriter;
  }

  public void DisableInterceptor(Type interceptorType)
  {
    _interceptorTypes ??= new();
    _interceptorTypes[interceptorType] = false;
  }

  public void EnableInterceptor(Type interceptorType)
  {
    _interceptorTypes ??= new();
    _interceptorTypes[interceptorType] = true;
  }

  public override object Execute(Expression expression)
  {
    expression = TryRewrite(expression);
    return base.Execute(expression);
  }

  public override TResult Execute<TResult>(Expression expression)
  {
    expression = TryRewrite(expression);
    return base.Execute<TResult>(expression);
  }

  public override TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
  {
    expression = TryRewrite(expression);
    return base.ExecuteAsync<TResult>(expression, cancellationToken);
  }

  private Expression TryRewrite(Expression expression)
  {
    return _rewriter.ShouldRewrite(_interceptorTypes)
      ? _rewriter.Rewrite(expression, _interceptorTypes)
      : expression;
  }
}
