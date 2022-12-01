namespace CodeArchitects.Platform.Data.AdoNet.Model;

public interface ISimpleNavigationModel : INavigationModel
{
  IReadOnlyList<IKeyPair> KeyPairs { get; }
}
