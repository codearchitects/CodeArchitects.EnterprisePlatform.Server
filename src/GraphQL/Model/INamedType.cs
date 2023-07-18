namespace CodeArchitects.Platform.GraphQL.Model;

public interface INamedType : IType
{
  string Name { get; }
}
