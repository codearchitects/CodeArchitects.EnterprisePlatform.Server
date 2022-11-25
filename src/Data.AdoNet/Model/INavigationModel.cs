namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface INavigationModel
{
  int Id { get; }
  bool IsOnDependent { get; }
  string Name { get; }
  IEntityModel From { get; }
  IEntityModel To { get; }
  IReadOnlyList<IKeyPair> Keys { get; }
}
