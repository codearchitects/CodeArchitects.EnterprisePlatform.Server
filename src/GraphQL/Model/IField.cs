namespace CodeArchitects.Platform.GraphQL.Model;

internal interface IField
{
  string Name { get; }

  Getter<object?> GetValue { get; }

  Setter<object?> SetValue { get; }

  IType Type { get; }
}
