using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface INavigationModel : IMemberModel
{
  int Id { get; }

  AssociationKind AssociationKind { get; }

  bool IsOnDependent { get; }

  bool IsCollection { get; }
  
  IEntityModel From { get; }
  
  IEntityModel To { get; }

  INavigationModel Inverse { get; }

  CollectionKind CollectionKind { get; }
}
