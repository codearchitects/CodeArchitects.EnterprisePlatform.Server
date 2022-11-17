using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Helpers;

public class ExpressionHelperTests
{
  [Fact]
  public void MakeShadowPropertyAccess_ShouldReturnCorrectExpression()
  {
    // Arrange
    const string propertyName = "Id";
    ParameterExpression entity = Expression.Parameter(typeof(Entity), nameof(entity));

    Expression<Func<Entity, Guid>> accessExpression = entity => EF.Property<Guid>(entity, propertyName);
    Expression expected = accessExpression.Body;

    // Act
    Expression shadowPropertyAccess = ExpressionHelpers.MakeShadowPropertyAccess(entity, propertyName, typeof(Guid));

    // Assert
    shadowPropertyAccess.Should().BeEquivalentTo(expected, options => options.Using(ExpressionEqualityComparer.Instance));
  }

  class Entity { }
}
