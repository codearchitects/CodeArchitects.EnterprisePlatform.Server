using System.Reflection;

namespace CodeArchitects.Platform.GraphQL.Model;

public interface IVariable
{
  string Name { get; }

  PropertyInfo ClrProperty { get; }

  IType Type { get; }
}
