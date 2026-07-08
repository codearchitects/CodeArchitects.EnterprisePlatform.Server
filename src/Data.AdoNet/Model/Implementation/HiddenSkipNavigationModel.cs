using CodeArchitects.Platform.Data.Features.Associations;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class HiddenSkipNavigationModel : SkipNavigationModel
{
  private readonly HiddenMemberComponent<object?> _memberComponent;

  public HiddenSkipNavigationModel(HiddenMemberComponent<object?> memberComponent, int id, EntityModel from, EntityModel to, AssociationKind associationKind, CollectionKind collectionKind, bool isOnDependent, JoinEntityModel joinEntity)
    : base(id, from, to, associationKind, collectionKind, isOnDependent, joinEntity)
  {
    _memberComponent = memberComponent;
  }

  protected override MemberComponent<object?> MemberComponent => _memberComponent;
}
