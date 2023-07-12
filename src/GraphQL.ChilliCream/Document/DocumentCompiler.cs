using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.GraphQL.Buffers;
using CodeArchitects.Platform.GraphQL.Document;
using StrawberryShake;

namespace CodeArchitects.Platform.GraphQL.ChilliCream.Document;

internal class DocumentCompiler : DocumentCompiler<Utf8Document>
{
  public DocumentCompiler(IBufferProvider bufferProvider, DocumentSerializationOptions options)
    : base(bufferProvider, options)
  {
  }

  protected override Utf8Document CreateDocument(OperationType operationType, string? name, byte[] content)
  {
    OperationKind kind = operationType switch
    {
      OperationType.Query        => OperationKind.Query,
      OperationType.Mutation     => OperationKind.Mutation,
      OperationType.Subscription => OperationKind.Subscription,
      _                          => throw Errors.Unreachable
    };

    return new(
      kind,
      name ?? "",
      content,
      Guid.NewGuid().ToString("N"));
  }
}
