namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class SimpleNavigationModel : NavigationModel, ISimpleNavigationModel
{
  private SimpleNavigationModel? _inverse;

  public SimpleNavigationModel(int id, IEntityModel from, IEntityModel to, AssociationKind associationKind, CollectionKind collectionKind, bool isOnDependent)
    : base(id, from, to, associationKind, collectionKind, isOnDependent)
  {
  }

  public IReadOnlyList<IKeyPair> KeyPairs => throw new NotImplementedException();

  public new SimpleNavigationModel Inverse
  {
    get => _inverse ?? throw new InvalidOperationException("Inverse navigation was not set.");
    set => _inverse = value;
  }

  public override INavigationModel InverseCore => Inverse;

  ISimpleNavigationModel ISimpleNavigationModel.Inverse => Inverse;
}
