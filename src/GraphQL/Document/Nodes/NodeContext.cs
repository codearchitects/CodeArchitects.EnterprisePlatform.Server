using CodeArchitects.Platform.GraphQL.Model;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

internal class NodeContext : INodeContext
{
  public bool TryGetDefaultSelection(Type fieldType, [NotNullWhen(true)] out LambdaExpression? selection)
  {
    throw new NotImplementedException();
  }

  public bool TryGetObjectType(Type type, [NotNullWhen(true)] out IObjectType? objectType)
  {
    throw new NotImplementedException();
  }
}
