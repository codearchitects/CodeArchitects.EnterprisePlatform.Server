namespace CodeArchitects.Platform.GraphQL.Model;

public interface IModel
{
  IType GetType(Type type);

  IReadOnlyList<IVariable> GetVariables(Type variablesType);
}
