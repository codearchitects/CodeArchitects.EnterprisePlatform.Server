using CodeArchitects.Platform.GraphQL.Document.Content;
using CodeArchitects.Platform.GraphQL.Document.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using Microsoft.Extensions.ObjectPool;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

internal partial class DocumentBuilder<TDocumentRoot, TSymbol> : IDocumentBuilder<TDocumentRoot>
  where TDocumentRoot : notnull
{
  private readonly INodeContext _nodeContext;
  private readonly IModel _model;
  private readonly DocumentBuilderOptions _options;
  private readonly ObjectPool<IDocumentContentBuilder<TSymbol>> _contentBuilderPool;

  public DocumentBuilder(INodeContext nodeContext, IModel model, DocumentBuilderOptions options, ObjectPool<IDocumentContentBuilder<TSymbol>> contentBuilderPool)
  {
    _nodeContext = nodeContext;
    _model = model;
    _options = options;
    _contentBuilderPool = contentBuilderPool;
  }

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
    IDocumentContentBuilder<TSymbol> contentBuilder = _contentBuilderPool.Get();
    OperationAppender<TSymbol> appender = new(contentBuilder, _options);
    appender.AppendOperationDefinition(operationDefinition);

    GraphDocument<TResult, TVariables> document = contentBuilder.GetDocument<TResult, TVariables>();
    _contentBuilderPool.Return(contentBuilder);

    return document;
  }
}
