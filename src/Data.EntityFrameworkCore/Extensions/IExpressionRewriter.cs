using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

internal interface IExpressionRewriter
{
  bool ShouldRewrite(IReadOnlyDictionary<Type, bool>? interceptorTypes);
  Expression Rewrite(Expression expression, IReadOnlyDictionary<Type, bool>? interceptorTypes);
}
