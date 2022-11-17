using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Helpers;

internal class TupleReplacer : ExpressionVisitor
{
  private int _index;
  private readonly ITuple _tuple;

  public TupleReplacer(ITuple tuple)
  {
    _tuple = tuple;
  }

  protected override Expression VisitLambda<T>(Expression<T> node)
  {
    Expression result = Expression.Lambda<T>(Visit(node.Body), node.Parameters);
    _index = 0;
    return result;
  }

  protected override Expression VisitBinary(BinaryExpression node)
  {
    if (node.NodeType == ExpressionType.AndAlso)
      return Expression.AndAlso(Visit(node.Left), Visit(node.Right));

    if (node.NodeType == ExpressionType.Equal)
      return Expression.Equal(node.Left, Visit(node.Right));

    return node;
  }

  protected override Expression VisitParameter(ParameterExpression node)
  {
    return Expression.Constant(_tuple[_index++], node.Type);
  }


  public static Expression Replace(Expression template, ITuple tuple)
  {
    return new TupleReplacer(tuple).Visit(template);
  }
}
