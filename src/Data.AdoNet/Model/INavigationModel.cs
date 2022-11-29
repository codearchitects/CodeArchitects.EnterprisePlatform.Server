namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface INavigationModel : INavigationModelBase
{
  bool IsOnDependent { get; }
  IReadOnlyList<IKeyPair> Keys { get; }
}
