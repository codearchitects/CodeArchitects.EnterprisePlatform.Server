using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface ISkipNavigationModel : INavigationModel
{
  IEntityModel JoinEntity { get; }
  
  IReadOnlyList<IKeyPair> FromKeys { get; }

  IReadOnlyList<IKeyPair> ToKeys { get; }

  new ISkipNavigationModel Inverse { get; }

  object CreateJoin(object from, object to);
}
