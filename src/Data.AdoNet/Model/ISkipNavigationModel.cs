namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface ISkipNavigationModel : INavigationModelBase
{
  string JoinTableName { get; }
  IReadOnlyList<IKeyPair> FromKeys { get; }
  IReadOnlyList<IKeyPair> ToKeys { get; }
}
