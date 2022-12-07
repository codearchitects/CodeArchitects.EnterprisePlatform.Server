namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class SkipNavigationModel : NavigationModel, ISkipNavigationModel
{
  protected SkipNavigationModel(int id, IEntityModel from, IEntityModel to, AssociationKind associationKind, CollectionKind collectionKind, bool isOnDependent)
    : base(id, from, to, associationKind, collectionKind, isOnDependent)
  {
  }

  public override INavigationModel InverseCore => Inverse;

  public new ISkipNavigationModel Inverse => throw new NotImplementedException();

  public IEntityModel JoinEntity => throw new NotImplementedException();

  public IReadOnlyList<IKeyPair> FromKeys => throw new NotImplementedException();

  public IReadOnlyList<IKeyPair> ToKeys => throw new NotImplementedException();

  public object CreateJoin(object from, object to)
  {
    throw new NotImplementedException();
  }
}
