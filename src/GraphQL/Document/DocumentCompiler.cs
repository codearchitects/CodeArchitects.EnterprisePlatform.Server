using CodeArchitects.Platform.GraphQL.Buffers;
using CodeArchitects.Platform.GraphQL.Document.Builder;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Buffers;

namespace CodeArchitects.Platform.GraphQL.Document;

internal abstract class DocumentCompiler<TUtf8Document> : IDocumentCompiler<TUtf8Document>
  where TUtf8Document : IUtf8Document
{
  private readonly BufferProvider _bufferProvider;
  private readonly DocumentSerializationOptions _options;

  public DocumentCompiler(BufferProvider bufferProvider, DocumentSerializationOptions options)
  {
    _bufferProvider = bufferProvider;
    _options = options;
  }

  protected abstract TUtf8Document CreateDocument(OperationType operationType, string? name, byte[] content);

  public TUtf8Document Compile(IOperationDefinitionNode operationDefinition)
  {
    byte[] content;

    using (BufferOwner owner = _bufferProvider.GetBuffer())
    {
      ArrayBufferWriter<byte> writer = owner.Writer;

      DocumentRenderer renderer = new(writer, _options);
      renderer.AppendOperationDefinition(operationDefinition);

      content = new byte[writer.WrittenCount];
      writer.WrittenSpan.CopyTo(content);
    }

    return CreateDocument(operationDefinition.OperationType, operationDefinition.Name, content);
  }
}
