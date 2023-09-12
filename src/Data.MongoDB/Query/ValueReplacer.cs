using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.MongoDB.Query;

internal class ValueReplacer : ExpressionVisitor
{
  private readonly object? _value;

  private ValueReplacer(object? value)
  {
    _value = value;
  }

  protected override Expression VisitLambda<T>(Expression<T> node)
  {
    return Expression.Lambda<T>(Visit(node.Body), node.Parameters);
  }

  protected override Expression VisitBinary(BinaryExpression node)
  {
    if (node.NodeType == ExpressionType.Equal)
      return Expression.Equal(node.Left, Visit(node.Right));

    return node;
  }

  protected override Expression VisitParameter(ParameterExpression node)
  {
    try
    {
      return Expression.Constant(_value, node.Type);
    }
    catch
    {
      throw;
    }
  }

  public static Expression Replace(Expression template, object? value)
  {
    return new ValueReplacer(value).Visit(template);
  }
}
