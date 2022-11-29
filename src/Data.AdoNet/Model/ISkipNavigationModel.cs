namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface ISkipNavigationModel : INavigationModel
{
  string JoinTableName { get; }
  IReadOnlyList<IKeyPair> FromKeys { get; }
  IReadOnlyList<IKeyPair> ToKeys { get; }
}
