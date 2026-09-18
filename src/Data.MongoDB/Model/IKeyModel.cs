namespace CodeArchitects.Platform.Data.MongoDB.Model;

internal interface IKeyModel
{
  string Name { get; }
  
  Type Type { get; }
  
  string ElementName { get; } // The name of the BSON element: "_id"
}
