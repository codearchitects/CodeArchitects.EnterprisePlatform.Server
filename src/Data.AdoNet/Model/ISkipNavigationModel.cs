namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface ISkipNavigationModel : INavigationModel
{
  IEntityModel JoinEntity { get; }
  
  IReadOnlyList<IKeyPair> FromKeys { get; }

  IReadOnlyList<IKeyPair> ToKeys { get; }

  object CreateJoin(object from, object to);
}
