using CodeArchitects.Platform.GraphQL.Buffers;
using CodeArchitects.Platform.GraphQL.Document.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using System.Buffers;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

internal abstract partial class DocumentBuilder<TDocumentRoot> : IDocumentBuilder<TDocumentRoot>
  where TDocumentRoot : notnull
{
  private readonly INodeContext _nodeContext;
  private readonly IModel _model;
  private readonly DocumentBuilderOptions _options;
  private readonly IBufferProvider _bufferProvider;

  public DocumentBuilder(INodeContext nodeContext, IModel model, DocumentBuilderOptions options, IBufferProvider bufferProvider)
  {
    _nodeContext = nodeContext;
    _model = model;
    _options = options;
    _bufferProvider = bufferProvider;
  }

  protected abstract GraphDocument<TResult, TVariables> CreateDocument<TResult, TVariables>(OperationType type, string? name, byte[] content)
    where TVariables : notnull;

  private IGraphDocument<TResult> BuildQuery<TResult>(string? name, Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion)
  {
    IReadOnlyCollection<IVariable> variables = Array.Empty<IVariable>();
    IOperationDefinitionNode operationDefinition = new QueryDefinitionNode(_nodeContext, name, variables, expansion.Body);

    return BuildDocument<TResult, EmptyVariables>(operationDefinition);
  }

  private IGraphDocument<TResult, TVariables> BuildQuery<TResult, TVariables>(string? name, Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull
  {
    IReadOnlyCollection<IVariable> variables = _model.GetVariables(typeof(TVariables));
    IOperationDefinitionNode operationDefinition = new QueryDefinitionNode(_nodeContext, name, variables, expansion.Body);

    return BuildDocument<TResult, TVariables>(operationDefinition);
  }

  private IGraphDocument<TResult> BuildMutation<TResult>(string? name, Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion)
  {
    IReadOnlyCollection<IVariable> variables = Array.Empty<IVariable>();
    IOperationDefinitionNode operationDefinition = new MutationDefinitionNode(_nodeContext, name, variables, expansion.Body);

    return BuildDocument<TResult, EmptyVariables>(operationDefinition);
  }

  private IGraphDocument<TResult, TVariables> BuildMutation<TResult, TVariables>(string? name, Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull
  {
    IReadOnlyCollection<IVariable> variables = _model.GetVariables(typeof(TVariables));
    IOperationDefinitionNode operationDefinition = new MutationDefinitionNode(_nodeContext, name, variables, expansion.Body);

    return BuildDocument<TResult, TVariables>(operationDefinition);
  }

  private GraphDocument<TResult, TVariables> BuildDocument<TResult, TVariables>(IOperationDefinitionNode operationDefinition)
    where TVariables : notnull
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

    return CreateDocument<TResult, TVariables>(operationDefinition.OperationType, operationDefinition.Name, content);
  }
}
