#pragma warning disable EF1001 // Internal EF Core API usage.

using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Linq;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

public class ExpressionRewriterTests
{
  private readonly IEntityType _entityType;
  private readonly Mock<IModel> _modelMock;
  private readonly Mock<IQueryRootExpressionInterceptor> _interceptorMock;
  private readonly Mock<IInterceptorInfo> _interceptorInfoMock;
  private readonly ExpressionRewriter _sut;

  public ExpressionRewriterTests()
  {
    _modelMock = new(MockBehavior.Strict);
    _interceptorMock = new(MockBehavior.Loose);
    IDbContextServices services = Mock.Of<IDbContextServices>(c => c.Model == _modelMock.Object);
    _sut = new(services, new[] { _interceptorMock.Object });

    _entityType = Mock.Of<IEntityType>(type => type.ClrType == typeof(Entity), MockBehavior.Strict);
    _modelMock
      .Setup(x => x.FindEntityType(typeof(Entity)))
      .Returns(_entityType);

    _interceptorInfoMock = new(MockBehavior.Strict);
  }

  [Fact]
  public void Rewrite_ShouldCallInterceptorsWithRootExpression_WhenTheyShouldApply()
  {
    // Arrange
    EntityQueryProvider provider = new(Mock.Of<IQueryCompiler>(MockBehavior.Strict));
    var queryable = new EntityQueryable<Entity>(provider, _entityType);

    // Explicitly cast to IQueryable<Entity> to resolve ambiguity
    var query = ((IQueryable<Entity>)queryable)
      .Where(x => x.Id > 4)
      .OrderBy(x => x.Id)
      .Select(x => new { NewId = x.Id });

    _interceptorMock
      .Setup(x => x.Apply(It.IsAny<Expression>(), It.IsAny<IEntityType>()))
      .Returns<Expression, IEntityType>((expr, type) => expr);

    _interceptorInfoMock
      .Setup(x => x.IsEnabled(It.IsAny<IQueryRootExpressionInterceptor>()))
      .Returns(true);

    Expression expression = query.Expression;

    // Act
    Expression result = _sut.Rewrite(expression, _interceptorInfoMock.Object);

    // Assert
    result.Should().BeEquivalentTo(expression, options => options.Using(ExpressionEqualityComparer.Instance)); // Because the interceptor is no-op
    _interceptorInfoMock.Verify(x => x.IsEnabled(_interceptorMock.Object));
    _interceptorMock.Verify(x => x.Apply(It.Is<EntityQueryRootExpression>(expr => expr.Type == typeof(IQueryable<Entity>)), _entityType));
    _interceptorMock.VerifyNoOtherCalls();
  }

  [Fact]
  public void Rewrite_ShouldNotCallInterceptorsWithRootExpression_WhenTheyShouldNotApply()
  {
    // Arrange
    EntityQueryProvider provider = new(Mock.Of<IQueryCompiler>(MockBehavior.Strict));
    var queryable = new EntityQueryable<Entity>(provider, _entityType);

    // Explicitly cast to IQueryable<Entity> to resolve ambiguity
    var query = ((IQueryable<Entity>)queryable)
      .Where(x => x.Id > 4)
      .OrderBy(x => x.Id)
      .Select(x => new { NewId = x.Id });

    _interceptorMock
      .Setup(x => x.Apply(It.IsAny<Expression>(), It.IsAny<IEntityType>()))
      .Returns<Expression, IEntityType>((expr, type) => expr);

    _interceptorInfoMock
      .Setup(x => x.IsEnabled(It.IsAny<IQueryRootExpressionInterceptor>()))
      .Returns(false);

    Expression expression = query.Expression;

    // Act
    Expression result = _sut.Rewrite(expression, _interceptorInfoMock.Object);

    // Assert
    result.Should().BeEquivalentTo(expression, options => options.Using(ExpressionEqualityComparer.Instance)); // Because the interceptor is no-op
    _interceptorInfoMock.Verify(x => x.IsEnabled(_interceptorMock.Object));
    _interceptorMock.VerifyNoOtherCalls();
  }

  class Entity
  {
    public int Id { get; set; }
  }
}
