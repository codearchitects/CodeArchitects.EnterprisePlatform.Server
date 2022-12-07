namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class SimpleNavigationModel : NavigationModel, ISimpleNavigationModel
{
  public SimpleNavigationModel(int id, IEntityModel from, IEntityModel to, AssociationKind associationKind, CollectionKind collectionKind, bool isOnDependent)
    : base(id, from, to, associationKind, collectionKind, isOnDependent)
  {
  }

  public IReadOnlyList<IKeyPair> KeyPairs => throw new NotImplementedException();

  public new ISimpleNavigationModel Inverse => throw new NotImplementedException();

  public override INavigationModel InverseCore => Inverse;
}
