using CodeArchitects.Platform.GraphQL.Document.Builder.Content;
using CodeArchitects.Platform.GraphQL.Document.Builder.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Builder.Nodes;
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
  private readonly ObjectPool<IContentBuilder<TSymbol>> _contentBuilderPool;

  public DocumentBuilder(INodeContext nodeContext, IModel model, DocumentBuilderOptions options, ObjectPool<IContentBuilder<TSymbol>> contentBuilderPool)
  {
    _nodeContext = nodeContext;
    _model = model;
    _options = options;
    _contentBuilderPool = contentBuilderPool;
  }

  private GraphDocument<TResult> BuildQuery<TResult>(string? name, Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion)
  {
    IReadOnlyCollection<IVariable> variables = Array.Empty<IVariable>();
    IOperationDefinitionNode operationDefinition = new QueryDefinitionNode(_nodeContext, name, variables, expansion.Body);

    return BuildDocument<TResult>(operationDefinition);
  }

  private GraphDocument<TResult, TVariables> BuildQuery<TResult, TVariables>(string? name, Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull
  {
    IReadOnlyCollection<IVariable> variables = _model.GetVariables(typeof(TVariables));
    IOperationDefinitionNode operationDefinition = new QueryDefinitionNode(_nodeContext, name, variables, expansion.Body);

    return BuildDocument<TResult, TVariables>(operationDefinition);
  }

  private GraphDocument<TResult> BuildMutation<TResult>(string? name, Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion)
  {
    IReadOnlyCollection<IVariable> variables = Array.Empty<IVariable>();
    IOperationDefinitionNode operationDefinition = new MutationDefinitionNode(_nodeContext, name, variables, expansion.Body);

    return BuildDocument<TResult>(operationDefinition);
  }

  private GraphDocument<TResult, TVariables> BuildMutation<TResult, TVariables>(string? name, Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull
  {
    IReadOnlyCollection<IVariable> variables = _model.GetVariables(typeof(TVariables));
    IOperationDefinitionNode operationDefinition = new MutationDefinitionNode(_nodeContext, name, variables, expansion.Body);

    return BuildDocument<TResult, TVariables>(operationDefinition);
  }

  private GraphDocument<TResult> BuildDocument<TResult>(IOperationDefinitionNode operationDefinition)
  {
    IContentBuilder<TSymbol> contentBuilder = _contentBuilderPool.Get();
    DocumentContentBuilder<TSymbol> builder = new(contentBuilder, _options);
    builder.AppendOperationDefinition(operationDefinition);

    GraphDocument<TResult> document = contentBuilder.GetDocument<TResult>();
    _contentBuilderPool.Return(contentBuilder);

    return document;
  }

  private GraphDocument<TResult, TVariables> BuildDocument<TResult, TVariables>(IOperationDefinitionNode operationDefinition)
    where TVariables : notnull
  {
    IContentBuilder<TSymbol> contentBuilder = _contentBuilderPool.Get();
    DocumentContentBuilder<TSymbol> builder = new(contentBuilder, _options);
    builder.AppendOperationDefinition(operationDefinition);

    GraphDocument<TResult, TVariables> document = contentBuilder.GetDocument<TResult, TVariables>();
    _contentBuilderPool.Return(contentBuilder);

    return document;
  }
}
