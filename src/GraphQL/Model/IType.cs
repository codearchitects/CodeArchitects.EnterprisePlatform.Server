namespace CodeArchitects.Platform.GraphQL.Model;

internal interface IType
{
  string Name { get; }

  Type ClrType { get; }

  TypeKind Kind { get; }
}
