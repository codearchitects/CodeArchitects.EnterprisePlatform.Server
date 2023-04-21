namespace CodeArchitects.Platform.Data.MongoDB.Model;

internal interface IEntityModel
{
  string CollectionName { get; }

  Type Type { get; }
  
  IKeyModel Key { get; }
}
