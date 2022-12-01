namespace CodeArchitects.Platform.Data.AdoNet.Model;

public interface ISkipNavigationModel : INavigationModel
{
  IEntityModel JoinEntity { get; }
  
  IReadOnlyList<IKeyPair> FromKeys { get; }

  IReadOnlyList<IKeyPair> ToKeys { get; }

  object CreateJoin(object from, object to);
}
