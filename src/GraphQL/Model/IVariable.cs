using System.Reflection;

namespace CodeArchitects.Platform.GraphQL.Model;

internal interface IVariable
{
  string Name { get; }

  PropertyInfo ClrProperty { get; }

  IType Type { get; }
}
