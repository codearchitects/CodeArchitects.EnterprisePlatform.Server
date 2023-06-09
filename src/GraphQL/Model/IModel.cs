namespace CodeArchitects.Platform.GraphQL.Model;

internal interface IModel
{
  IType GetType(Type type);

  IReadOnlyCollection<IVariable> GetVariables(Type variablesType);
}
