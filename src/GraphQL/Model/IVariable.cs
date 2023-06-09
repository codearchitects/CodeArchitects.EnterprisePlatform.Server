namespace CodeArchitects.Platform.GraphQL.Model;

internal interface IVariable
{
  string Name { get; }

  IType Type { get; }
}
