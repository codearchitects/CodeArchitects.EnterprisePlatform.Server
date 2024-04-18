using CodeArchitects.Platform.PolicyManager.Models;
using CodeArchitects.Platform.PolicyManager.PredicateBuilder;
using System.Linq.Expressions;
using System.Security.Claims;

namespace CodeArchitects.Platform.PolicyManager.IntegrationTests;
public class ExpressionBuilderTests
{
	[Theory]
	[InlineData(PolicyConditionType.And, true, true, true)]
	[InlineData(PolicyConditionType.And, true, false, false)]
	[InlineData(PolicyConditionType.And, false, true, false)]
	[InlineData(PolicyConditionType.And, false, false, false)]
	[InlineData(PolicyConditionType.Or, true, true, true)]
	[InlineData(PolicyConditionType.Or, true, false, true)]
	[InlineData(PolicyConditionType.Or, false, true, true)]
	[InlineData(PolicyConditionType.Or, false, false, false)]
	public void Policy_Combination_Works_Correctly(PolicyConditionType conditionType, bool leftValue, bool rightValue, bool expectedResult)
	{
		// Arrange
		var leftExpr = Expression.Constant(leftValue);
		var rightExpr = Expression.Constant(rightValue);

		// Act
		var combinedExpr = conditionType switch
		{
			PolicyConditionType.And => ExpressionBuilder.AndAlso(Expression.Lambda<Func<ClaimsPrincipal, bool>>(leftExpr, Expression.Parameter(typeof(ClaimsPrincipal))),
																													Expression.Lambda<Func<ClaimsPrincipal, bool>>(rightExpr, Expression.Parameter(typeof(ClaimsPrincipal)))),
			PolicyConditionType.Or => ExpressionBuilder.OrElse(Expression.Lambda<Func<ClaimsPrincipal, bool>>(leftExpr, Expression.Parameter(typeof(ClaimsPrincipal))),
																												 Expression.Lambda<Func<ClaimsPrincipal, bool>>(rightExpr, Expression.Parameter(typeof(ClaimsPrincipal)))),
			_ => throw new InvalidOperationException("Unexpected policy condition type.")
		};

		// Compile and execute the expression
		var result = combinedExpr.Compile()(new ClaimsPrincipal());

		// Assert
		result.Should().Be(expectedResult);
	}
}
