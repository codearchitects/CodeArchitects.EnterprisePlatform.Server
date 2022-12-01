namespace CodeArchitects.Platform.Data.AdoNet.Model;

public interface INavigationModel : IPropertyModelBase
{
  int Id { get; }

  int Index { get; }

  bool IsAggregation { get; }

  bool IsOnDependent { get; }

  bool IsCollection { get; }
  
  IEntityModel From { get; }
  
  IEntityModel To { get; }

  IForeignKeyModel ForeignKey { get; }

  CollectionKind CollectionKind { get; }
}
