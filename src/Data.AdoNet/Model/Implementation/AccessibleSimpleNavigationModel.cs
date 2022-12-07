using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class AccessibleSimpleNavigationModel : SimpleNavigationModel, IAccessibleSimpleNavigationModel
{
  private readonly AccessibleMemberComponent _memberComponent;

  public AccessibleSimpleNavigationModel(AccessibleMemberComponent memberComponent, int id, IEntityModel from, IEntityModel to, AssociationKind associationKind, CollectionKind collectionKind, bool isOnDependent)
    : base(id, from, to, associationKind, collectionKind, isOnDependent)
  {
    _memberComponent = memberComponent;
  }

  protected override MemberComponent MemberComponent => _memberComponent;

  public new MemberInfo Member => _memberComponent.Member;

  public new Getter<object?> GetValue => _memberComponent.GetValue;

  public new Setter<object?> SetValue => _memberComponent.SetValue;
}
