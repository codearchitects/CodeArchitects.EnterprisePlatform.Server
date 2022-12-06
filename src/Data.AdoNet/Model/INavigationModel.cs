using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface INavigationModel : IMemberModelBase
{
  int Id { get; }

  int Index { get; }

  AssociationKind AssociationKind { get; }

  bool IsOnDependent { get; }

  bool IsCollection { get; }
  
  IEntityModel From { get; }
  
  IEntityModel To { get; }

  IPrimaryKeyModel PrimaryKey { get; }

  IForeignKeyModel ForeignKey { get; }

  INavigationModel Inverse { get; }

  CollectionKind CollectionKind { get; }
}
