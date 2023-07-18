using CodeArchitects.Platform.GraphQL.Buffers;
using CodeArchitects.Platform.GraphQL.Document;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Nodes.FluentMock;
using FluentAssertions;
using Microsoft.Extensions.ObjectPool;
using System.Buffers;
using System.Text;

namespace CodeArchitects.Platform.GraphQL.ChilliCream.Document;

public class DocumentCompilerTests
{
  private readonly DocumentCompiler _sut;

  public DocumentCompilerTests()
  {
    Mock<ObjectPool<ArrayBufferWriter<byte>>> poolMock = new(MockBehavior.Loose);
    poolMock
      .Setup(x => x.Get())
      .Returns(() => new ArrayBufferWriter<byte>());

    BufferProvider bufferProvider = new(poolMock.Object);
    DocumentSerializationOptions options = new()
    {
      LinePolicy = LinePolicy.Space(1)
    };

    _sut = new(bufferProvider, options);
  }

  [Theory]
  [InlineData(OperationType.Query, "query")]
  [InlineData(OperationType.Mutation, "mutation")]
  public void Compile_ShouldReturnCorrectDocument(OperationType operationType, string operation)
  {
    // Arrange
    string name = "test";

    IOperationDefinitionNode operationDefinition = OperationDefinitionNodeBuilder.Build(_ => _
      .SetOperationType(operationType)
      .SetName("test")
      .SetDirectiveList(null as IDirectiveListNode)
      .SetVariableDefinitionList(null as IVariableDefinitionListNode)
      .SetSelectionSet(_ => _
        .SetSelections()));

    // Act
    Utf8Document document = _sut.Compile(operationType, name, operationDefinition);

    // Assert
    document.Content.Should().BeEquivalentTo(Encoding.UTF8.GetBytes($"{operation} {name} {{  }}"));
  }
}
