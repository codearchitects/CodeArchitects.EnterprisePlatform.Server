using CodeArchitects.Platform.GraphQL.ChilliCream.Document;
using CodeArchitects.Platform.GraphQL.Document;
using StrawberryShake;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.ChilliCream;

public class GraphClientTests
{
  private const string s_name = "name";

  private readonly GraphClient<IDocumentRoot> _sut;

  public GraphClientTests()
  {
    Mock<IDocumentCache<Utf8Document>> documentCacheMock = new(MockBehavior.Strict);
    documentCacheMock
      .Setup(x => x.GetOrCompile(It.IsAny<GraphDocument>(), It.IsAny<Func<GraphDocument, Utf8Document>>()))
      .Returns<GraphDocument, Func<GraphDocument, Utf8Document>>((document, compileDocument) => compileDocument(document));

    IOperationExecutorProvider executorProvider = Mock.Of<IOperationExecutorProvider>(provider =>
      provider.GetExecutor<object>() == null!, MockBehavior.Strict);

    IVariableExtractorProvider extractorProvider = Mock.Of<IVariableExtractorProvider>(provider =>
      provider.GetExtractor<object>() == null!, MockBehavior.Strict);

    _sut = new(
      documentCacheMock.Object,
      graphDocument => new Utf8Document(OperationKind.Query, s_name, Array.Empty<byte>(), "id"),
      executorProvider,
      extractorProvider);
  }

  [Fact]
  public void X()
  {
    // Arrange
    GraphDocument<object, object> document = new GraphDocument<object, object>.Query(s_name, Expression.Constant(null));

    // Act
    IGraphRequest<object, object> request = _sut.Request(document);

    // Assert
  }
}
