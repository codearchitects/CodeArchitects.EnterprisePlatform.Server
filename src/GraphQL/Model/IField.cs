using System.Reflection;

namespace CodeArchitects.Platform.GraphQL.Model;

public interface IField
{
  string Name { get; }

  PropertyInfo ClrProperty { get; }

  Getter<object?> GetValue { get; }

  Setter<object?> SetValue { get; }

  IType Type { get; }
}
