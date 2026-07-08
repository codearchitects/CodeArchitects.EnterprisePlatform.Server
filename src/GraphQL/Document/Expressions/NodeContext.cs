using CodeArchitects.Platform.GraphQL.Model;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class NodeContext : INodeContext
{
  public IVariable GetVariable(PropertyInfo property)
  {
    throw new NotImplementedException();
  }

  public bool TryGetDefaultSelection(Type fieldType, [NotNullWhen(true)] out LambdaExpression? selection)
  {
    throw new NotImplementedException();
  }

  public bool TryGetObjectType(Type type, [NotNullWhen(true)] out IObjectType? objectType)
  {
    throw new NotImplementedException();
  }
}
