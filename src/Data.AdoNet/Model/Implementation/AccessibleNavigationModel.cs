using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class AccessibleNavigationModel : NavigationModel, IAccessibleNavigationModel
{
  private readonly AccessibleMemberComponent _memberComponent;

  public AccessibleNavigationModel(AccessibleMemberComponent memberComponent, int id, EntityModel from, EntityModel to, AssociationKind associationKind)
    : base(id, from, to, associationKind)
  {
    _memberComponent = memberComponent;
  }

  protected override MemberComponent MemberComponent => _memberComponent;

  public new MemberInfo Member => _memberComponent.Member;

  public new Getter<object?> GetValue => _memberComponent.GetValue;

  public new Setter<object?> SetValue => _memberComponent.SetValue;
}
