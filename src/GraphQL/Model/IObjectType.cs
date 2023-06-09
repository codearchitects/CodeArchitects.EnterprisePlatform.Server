namespace CodeArchitects.Platform.GraphQL.Model;

internal interface IObjectType : IType // TODO: Nullability
{
  IReadOnlyList<IField> Fields { get; }
}
