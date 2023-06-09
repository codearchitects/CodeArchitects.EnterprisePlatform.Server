using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class MutationDefinitionNode : OperationDefinitionNode
{
  public MutationDefinitionNode(INodeContext context, string? name, IEnumerable<IVariable> variables, Expression expression)
    : base(context, name, variables, expression)
  {
  }

  public override OperationType OperationType => OperationType.Mutation;
}
