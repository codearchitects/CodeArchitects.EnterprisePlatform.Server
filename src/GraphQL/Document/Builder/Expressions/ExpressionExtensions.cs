using CodeArchitects.Platform.Common.Expressions;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder.Expressions;

internal static class ExpressionExtensions
{
  public static LambdaExpression EvaluateAsLambda(this Expression expression)
  {
    if (expression.NodeType is ExpressionType.Quote) // This is when the expression is passed inline
      return (LambdaExpression)((UnaryExpression)expression).Operand;

    return ExpressionEvaluator.Evaluate<LambdaExpression>(expression);
  }
}
