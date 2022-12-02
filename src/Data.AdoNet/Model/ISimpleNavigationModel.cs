using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface ISimpleNavigationModel : INavigationModel
{
  IReadOnlyList<IKeyPair> KeyPairs { get; }

  new ISimpleNavigationModel Inverse { get; }
}
