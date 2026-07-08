using CodeArchitects.Platform.GraphQL.Model;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal interface INodeContext
{
  bool TryGetDefaultSelection(Type fieldType, [NotNullWhen(true)] out LambdaExpression? selection);

  bool TryGetObjectType(Type type, [NotNullWhen(true)] out IObjectType? objectType);

  IVariable GetVariable(PropertyInfo property);
}
