using CodeArchitects.Platform.GraphQL.Model;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class MutationDocumentNode : DocumentNode
{
  public MutationDocumentNode(INodeContext context, string name, IReadOnlyList<IVariable> variables, Expression expression)
    : base(context, name, variables, expression)
  {
  }

  protected override OperationDefinitionNode CreateOperationDefinition(string name, IReadOnlyList<IVariable> variables, Expression expression)
  {
    return new MutationDefinitionNode(this, name, variables, expression);
  }
}
