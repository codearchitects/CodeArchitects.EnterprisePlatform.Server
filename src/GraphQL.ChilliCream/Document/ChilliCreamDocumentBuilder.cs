using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.GraphQL.Buffers;
using CodeArchitects.Platform.GraphQL.Document;
using CodeArchitects.Platform.GraphQL.Document.Builder;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using StrawberryShake;

namespace CodeArchitects.Platform.GraphQL.ChilliCream.Document;

internal class ChilliCreamDocumentBuilder<TDocumentRoot> : DocumentBuilder<TDocumentRoot>
  where TDocumentRoot : notnull
{
  public ChilliCreamDocumentBuilder(INodeContext nodeContext, IModel model, DocumentBuilderOptions options, IBufferProvider bufferProvider)
    : base(nodeContext, model, options, bufferProvider)
  {
  }

  protected override GraphDocument<TResult, TVariables> CreateDocument<TResult, TVariables>(OperationType type, string? name, byte[] content)
  {
    OperationKind kind = type switch
    {
      OperationType.Query        => OperationKind.Query,
      OperationType.Mutation     => OperationKind.Mutation,
      OperationType.Subscription => OperationKind.Subscription,
      _                          => throw Errors.Unreachable
    };

    return new ChilliCreamGraphDocument<TResult, TVariables>(
      kind,
      name ?? string.Empty,
      content,
      Guid.NewGuid().ToString("N"));
  }
}
