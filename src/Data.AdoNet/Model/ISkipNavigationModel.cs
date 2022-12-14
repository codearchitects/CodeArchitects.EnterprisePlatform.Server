using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface ISkipNavigationModel : INavigationModel
{
  IEntityModel JoinEntity { get; }
  
  IReadOnlyList<IKeyPair> FromKeyPairs { get; }

  IReadOnlyList<IKeyPair> ToKeyPairs { get; }

  new ISkipNavigationModel Inverse { get; }

  object CreateJoin(object from, object to);
}
