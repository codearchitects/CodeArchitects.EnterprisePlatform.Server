namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class SkipNavigationModel : NavigationModel, ISkipNavigationModel
{
  private SkipNavigationModel? _inverse;

  protected SkipNavigationModel(int id, IEntityModel from, IEntityModel to, AssociationKind associationKind, CollectionKind collectionKind, bool isOnDependent, IEntityModel joinEntity)
    : base(id, from, to, associationKind, collectionKind, isOnDependent)
  {
    JoinEntity = joinEntity;
  }

  public override INavigationModel InverseCore => Inverse;

  public new SkipNavigationModel Inverse
  {
    get => _inverse ?? throw new InvalidOperationException("Inverse navigation was not set.");
    set => _inverse = value;
  }

  public IEntityModel JoinEntity { get; }

  public IReadOnlyList<IKeyPair> FromKeys => throw new NotImplementedException();

  public IReadOnlyList<IKeyPair> ToKeys => throw new NotImplementedException();

  ISkipNavigationModel ISkipNavigationModel.Inverse => Inverse;

  public object CreateJoin(object from, object to)
  {
    throw new NotImplementedException();
  }
}
