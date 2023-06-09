using CodeArchitects.Platform.GraphQL.Document.Builder.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Builder.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using Microsoft.Extensions.ObjectPool;
using System.Linq.Expressions;
using System.Text;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

internal partial class DocumentBuilder<TDocumentRoot> : IDocumentBuilder<TDocumentRoot>
  where TDocumentRoot : notnull
{
  private readonly INodeContext _nodeContext;
  private readonly IModel _model;
  private readonly DocumentBuilderOptions _options;
  private readonly ObjectPool<StringBuilder> _stringBuilderPool;

  public DocumentBuilder(INodeContext nodeContext, IModel model, DocumentBuilderOptions options, ObjectPool<StringBuilder> stringBuilderPool)
  {
    _nodeContext = nodeContext;
    _model = model;
    _options = options;
    _stringBuilderPool = stringBuilderPool;
  }

  private GraphDocument<TField> BuildQuery<TField>(string? name, Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TField>>> expansion)
  {
    IReadOnlyCollection<IVariable> variables = Array.Empty<IVariable>();
    string content = BuildContent(new QueryDefinitionNode(_nodeContext, name, variables, expansion.Body));

    return new GraphDocument<TField>(OperationType.Query, content);
  }

  private GraphDocument<TField, TVariables> BuildQuery<TField, TVariables>(string? name, Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TField, TVariables>>> expansion)
    where TVariables : notnull
  {
    IReadOnlyCollection<IVariable> variables = _model.GetVariables(typeof(TVariables));
    string content = BuildContent(new QueryDefinitionNode(_nodeContext, name, variables, expansion.Body));

    return new GraphDocument<TField, TVariables>(OperationType.Query, content);
  }

  private GraphDocument<TField> BuildMutation<TField>(string? name, Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TField>>> expansion)
  {
    IReadOnlyCollection<IVariable> variables = Array.Empty<IVariable>();
    string content = BuildContent(new MutationDefinitionNode(_nodeContext, name, variables, expansion.Body));

    return new GraphDocument<TField>(OperationType.Mutation, content);
  }

  private GraphDocument<TField, TVariables> BuildMutation<TField, TVariables>(string? name, Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TField, TVariables>>> expansion)
    where TVariables : notnull
  {
    IReadOnlyCollection<IVariable> variables = _model.GetVariables(typeof(TVariables));
    string content = BuildContent(new MutationDefinitionNode(_nodeContext, name, variables, expansion.Body));

    return new GraphDocument<TField, TVariables>(OperationType.Mutation, content);
  }

  private string BuildContent(IOperationDefinitionNode operationDefinition)
  {
    StringBuilder sb = _stringBuilderPool.Get();
    DocumentStringBuilder documentStringBuilder = new(sb, _options);
    documentStringBuilder.AppendOperationDefinition(operationDefinition);

    string content = sb.ToString();
    _stringBuilderPool.Return(sb);

    return content;
  }
}
