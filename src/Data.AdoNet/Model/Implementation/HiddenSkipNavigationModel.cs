namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class HiddenSkipNavigationModel : SkipNavigationModel
{
  private readonly HiddenMemberComponent<object?> _memberComponent;

  public HiddenSkipNavigationModel(HiddenMemberComponent<object?> memberComponent, int id, IEntityModel from, IEntityModel to, AssociationKind associationKind, CollectionKind collectionKind, bool isOnDependent, IEntityModel joinEntity)
    : base(id, from, to, associationKind, collectionKind, isOnDependent, joinEntity)
  {
    _memberComponent = memberComponent;
  }

  protected override MemberComponent<object?> MemberComponent => _memberComponent;
}
