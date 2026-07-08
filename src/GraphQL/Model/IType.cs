namespace CodeArchitects.Platform.GraphQL.Model;

public interface IType
{
  bool IsNullable { get; }

  Type ClrType { get; }

  TypeKind Kind { get; }
}
