using CodeArchitects.Platform.GraphQL.Model;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

internal interface INodeContext
{
  bool TryGetDefaultSelection(Type fieldType, [NotNullWhen(true)] out LambdaExpression? selection);

  bool TryGetObjectType(Type type, [NotNullWhen(true)] out IObjectType? objectType);
}
