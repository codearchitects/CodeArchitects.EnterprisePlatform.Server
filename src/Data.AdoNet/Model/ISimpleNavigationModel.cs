using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface ISimpleNavigationModel : INavigationModel
{
  IPrimaryKeyModel PrimaryKey { get; }

  IReadOnlyList<IKeyPair> KeyPairs { get; }

  new ISimpleNavigationModel Inverse { get; }
}
