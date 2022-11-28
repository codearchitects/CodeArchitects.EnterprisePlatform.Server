namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface INavigationModel : IPropertyBase
{
  int Id { get; }
  int Index { get; }
  bool IsOnDependent { get; }
  bool IsCollection { get; }
  IEntityModel From { get; }
  IEntityModel To { get; }
  IReadOnlyList<IKeyPair> Keys { get; }
}
