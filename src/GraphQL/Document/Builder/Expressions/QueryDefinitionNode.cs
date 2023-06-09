using CodeArchitects.Platform.GraphQL.Model;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder.Expressions;

internal class QueryDefinitionNode : OperationDefinitionNode
{
  public QueryDefinitionNode(INodeContext context, string? name, IEnumerable<IVariable> variables, Expression expression)
    : base(context, name, variables, expression)
  {
  }

  public override OperationType OperationType => OperationType.Query;
}
