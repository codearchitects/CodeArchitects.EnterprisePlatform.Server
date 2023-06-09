using CodeArchitects.Platform.GraphQL.Document.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using Microsoft.Extensions.ObjectPool;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

internal abstract partial class DocumentBuilder<TDocumentRoot> : IDocumentBuilder<TDocumentRoot>
  where TDocumentRoot : notnull
{
  private readonly INodeContext _nodeContext;
  private readonly IModel _model;
  private readonly DocumentBuilderOptions _options;
  private readonly ObjectPool<MemoryStream> _memoryStreamPool;

  public DocumentBuilder(INodeContext nodeContext, IModel model, DocumentBuilderOptions options, ObjectPool<MemoryStream> memoryStreamPool)
  {
    _nodeContext = nodeContext;
    _model = model;
    _options = options;
    _memoryStreamPool = memoryStreamPool;
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
    MemoryStream ms = _memoryStreamPool.Get();
    OperationAppender appender = new(ms, _options);
    appender.AppendOperationDefinition(operationDefinition);

    GraphDocument<TResult, TVariables> document = CreateDocument<TResult, TVariables>(operationDefinition.OperationType, operationDefinition.Name, ms.ToArray());
    _memoryStreamPool.Return(ms);

    return document;
  }
}
