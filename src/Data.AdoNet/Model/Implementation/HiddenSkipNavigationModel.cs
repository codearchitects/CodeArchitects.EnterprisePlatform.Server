namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class HiddenSkipNavigationModel : SkipNavigationModel
{
  private readonly HiddenMemberComponent<object?> _memberComponent;

  public HiddenSkipNavigationModel(HiddenMemberComponent<object?> memberComponent, int id, IEntityModel from, IEntityModel to, AssociationKind associationKind, CollectionKind collectionKind, bool isOnDependent)
    : base(id, from, to, associationKind, collectionKind, isOnDependent)
  {
    _memberComponent = memberComponent;
  }

  protected override MemberComponent<object?> MemberComponent => _memberComponent;
}
