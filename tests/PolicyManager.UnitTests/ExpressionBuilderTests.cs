using CodeArchitects.Platform.PolicyManager.PredicateBuilder;
using System.Linq.Expressions;
using System.Security.Claims;

namespace CodeArchitects.Platform.PolicyManager;

public class ExpressionBuilderTests
{
	[Theory]
	[InlineData(true, true, true)]
	[InlineData(true, false, true)]
	[InlineData(false, true, true)]
	[InlineData(false, false, false)]
	public void AndAlso_Combines_Two_Expressions_With_AndAlso_Operator_Correctly(bool expr1Result, bool expr2Result, bool expectedResult)
	{
		// Arrange
		Expression<Func<ClaimsPrincipal, bool>> expr1 = principal => expr1Result;
		Expression<Func<ClaimsPrincipal, bool>> expr2 = principal => expr2Result;

		// Act
		var combinedExpr = ExpressionBuilder.OrElse(expr1, expr2);
		var compiled = combinedExpr.Compile();
		bool result = compiled(new ClaimsPrincipal());

		// Assert
		result.Should().Be(expectedResult);
	}

	[Theory]
	[InlineData(true, true, true)]
	[InlineData(true, false, true)]
	[InlineData(false, true, true)]
	[InlineData(false, false, false)]
	public void OrElse_Combines_Two_Expressions_With_OrElse_Operator_Correctly(bool expr1Result, bool expr2Result, bool expectedResult)
	{
		// Arrange
		Expression<Func<ClaimsPrincipal, bool>> expr1 = principal => expr1Result;
		Expression<Func<ClaimsPrincipal, bool>> expr2 = principal => expr2Result;

		// Act
		var combinedExpr = ExpressionBuilder.OrElse(expr1, expr2);
		var compiled = combinedExpr.Compile();
		bool result = compiled(new ClaimsPrincipal());

		// Assert
		result.Should().Be(expectedResult);
	}
}
