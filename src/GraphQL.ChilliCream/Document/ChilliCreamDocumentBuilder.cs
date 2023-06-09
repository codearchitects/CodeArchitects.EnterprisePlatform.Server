using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.GraphQL.Document;
using CodeArchitects.Platform.GraphQL.Document.Builder;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using Microsoft.Extensions.ObjectPool;
using StrawberryShake;

namespace CodeArchitects.Platform.GraphQL.ChilliCream.Document;

internal class ChilliCreamDocumentBuilder<TDocumentRoot> : DocumentBuilder<TDocumentRoot>
  where TDocumentRoot : notnull
{
  public ChilliCreamDocumentBuilder(INodeContext nodeContext, IModel model, DocumentBuilderOptions options, ObjectPool<MemoryStream> memoryStreamPool)
    : base(nodeContext, model, options, memoryStreamPool)
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
