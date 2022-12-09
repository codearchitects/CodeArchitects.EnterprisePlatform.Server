using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class AccessibleForeignKeyColumnModel : ForeignKeyColumnModel, IAccessibleColumnModel
{
  private readonly AccessibleMemberComponent<object?> _memberComponent;

  public AccessibleForeignKeyColumnModel(AccessibleMemberComponent<object?> memberComponent, short index, short foreignKeyIndex, INavigationModel navigation)
    : base(index, foreignKeyIndex, navigation)
  {
    _memberComponent = memberComponent;
  }

  protected override MemberComponent<object?> MemberComponent => _memberComponent;

  [AllowNull]
  public override string Name
  {
    get => _memberComponent.Name;
    set => _memberComponent.Name = value;
  }

  public new MemberInfo Member => _memberComponent.Member;

  public new Getter<object?> GetValue => _memberComponent.GetValue;

  public new Setter<object?> SetValue => _memberComponent.SetValue;
}
