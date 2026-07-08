namespace CodeArchitects.Platform.Data.MongoDB.Model;

internal interface IKeyModel
{
  string Name { get; }
  
  Type Type { get; }
}
