namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface INavigationModel : IPropertyModelBase
{
  int Id { get; }
  int Index { get; }
  bool IsCollection { get; }
  IEntityModel From { get; }
  IEntityModel To { get; }
}
