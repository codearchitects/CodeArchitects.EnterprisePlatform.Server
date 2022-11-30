namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface ISimpleNavigationModel : INavigationModel
{
  IReadOnlyList<IKeyPair> KeyPairs { get; }
}
