using System.Linq.Expressions;
using System.Security.Claims;

namespace CodeArchitects.Platform.PolicyManager.PredicateBuilder;

internal class ExpressionBuilder
{
	public static Expression<Func<ClaimsPrincipal, bool>> AndAlso(
		Expression<Func<ClaimsPrincipal, bool>> leftExpr,
		Expression<Func<ClaimsPrincipal, bool>> rightExpr)
	{
		var param = Expression.Parameter(typeof(ClaimsPrincipal), "subject");

		var leftVisitor = new ReplaceExpressionVisitor(leftExpr.Parameters[0], param);
		var left = leftVisitor.Visit(leftExpr.Body);

		var rightVisitor = new ReplaceExpressionVisitor(rightExpr.Parameters[0], param);
		var right = rightVisitor.Visit(rightExpr.Body);

		return Expression.Lambda<Func<ClaimsPrincipal, bool>>(
						Expression.AndAlso(left, right), param);
	}

	public static Expression<Func<ClaimsPrincipal, bool>> OrElse(
		Expression<Func<ClaimsPrincipal, bool>> leftExpr,
		Expression<Func<ClaimsPrincipal, bool>> rightExpr)
	{
		var param = Expression.Parameter(typeof(ClaimsPrincipal), "subject");

		var leftVisitor = new ReplaceExpressionVisitor(leftExpr.Parameters[0], param);
		var left = leftVisitor.Visit(leftExpr.Body);

		var rightVisitor = new ReplaceExpressionVisitor(rightExpr.Parameters[0], param);
		var right = rightVisitor.Visit(rightExpr.Body);

		return Expression.Lambda<Func<ClaimsPrincipal, bool>>(
						Expression.OrElse(left, right), param);
	}
}
