using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

internal interface IExpressionRewriter
{
  bool ShouldRewrite(IInterceptorInfo interceptorInfo);
  Expression Rewrite(Expression expression, IInterceptorInfo interceptorInfo);
}
