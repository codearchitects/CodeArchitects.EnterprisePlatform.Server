namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface ISimpleNavigationModel : INavigationModel
{
  bool IsOnDependent { get; }
  IReadOnlyList<IKeyPair> Keys { get; }
}
