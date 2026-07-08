namespace CodeArchitects.Platform.GraphQL.Model;

public interface IObjectType : INamedType
{
  IReadOnlyList<IField> Fields { get; }
}
