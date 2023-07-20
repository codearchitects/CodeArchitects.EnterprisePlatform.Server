using CodeArchitects.Platform.GraphQL.Model;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class QueryDefinitionNode : OperationDefinitionNode
{
  public QueryDefinitionNode(INodeRoot root, string name, IReadOnlyList<IVariable> variables, Expression expression)
    : base(root, name, variables, expression)
  {
  }

  public override OperationType OperationType => OperationType.Query;
}
