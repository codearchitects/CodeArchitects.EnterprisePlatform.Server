namespace CodeArchitects.Platform.GraphQL.Model;

public interface IListType : IType
{
  IType ItemType { get; }
}
