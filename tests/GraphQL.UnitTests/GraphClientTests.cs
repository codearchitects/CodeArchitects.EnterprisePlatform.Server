using CodeArchitects.Platform.GraphQL.Document;
using FluentAssertions;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL;

public class GraphClientTests
{
  private readonly Mock<IDocumentCache<IUtf8Document>> _documentCacheMock;
  private readonly Mock<IRequestFactory<IUtf8Document>> _requestFactoryMock;

  public GraphClientTests()
  {
    _documentCacheMock = new(MockBehavior.Strict);
    _documentCacheMock
      .Setup(x => x.GetOrCompile(It.IsAny<GraphDocument>(), It.IsAny<Func<GraphDocument, IUtf8Document>>()))
      .Returns<GraphDocument, Func<GraphDocument, IUtf8Document>>((document, compile) => compile(document));

    _requestFactoryMock = new(MockBehavior.Strict);
  }

  [Fact]
  public void RequestWithoutVariables_ShouldCallCorrectMethodsOnDependencies()
  {
    // Arrange
    IUtf8Document utf8Document = Mock.Of<IUtf8Document>(MockBehavior.Strict);
    IGraphRequest<object> expected = Mock.Of<IGraphRequest<object>>(MockBehavior.Strict);

    _requestFactoryMock
      .Setup(x => x.CreateRequest<object>(It.IsAny<IUtf8Document>()))
      .Returns(expected);

    GraphDocument<object> document = new GraphDocument<object>.Query(null, Expression.Constant(null));

    GraphClient<IUtf8Document, IDocumentRoot> sut = new(_documentCacheMock.Object, _ => utf8Document, _requestFactoryMock.Object);

    // Act
    IGraphRequest<object> actual = sut.Request(document);

    // Assert
    actual.Should().BeSameAs(expected);
    _documentCacheMock.Verify(x => x.GetOrCompile(document, It.IsAny<Func<GraphDocument, IUtf8Document>>()), Times.Once);
    _requestFactoryMock.Verify(x => x.CreateRequest<object>(utf8Document), Times.Once);
  }

  [Fact]
  public void RequestWithVariables_ShouldCallCorrectMethodsOnDependencies()
  {
    // Arrange
    IUtf8Document utf8Document = Mock.Of<IUtf8Document>(MockBehavior.Strict);
    IGraphRequest<object, object> expected = Mock.Of<IGraphRequest<object, object>>(MockBehavior.Strict);

    _requestFactoryMock
      .Setup(x => x.CreateRequest<object, object>(It.IsAny<IUtf8Document>()))
      .Returns(expected);

    GraphDocument<object, object> document = new GraphDocument<object, object>.Query(null, Expression.Constant(null));

    GraphClient<IUtf8Document, IDocumentRoot> sut = new(_documentCacheMock.Object, _ => utf8Document, _requestFactoryMock.Object);

    // Act
    IGraphRequest<object, object> actual = sut.Request(document);

    // Assert
    actual.Should().BeSameAs(expected);
    _documentCacheMock.Verify(x => x.GetOrCompile(document, It.IsAny<Func<GraphDocument, IUtf8Document>>()), Times.Once);
    _requestFactoryMock.Verify(x => x.CreateRequest<object, object>(utf8Document), Times.Once);
  }
}
