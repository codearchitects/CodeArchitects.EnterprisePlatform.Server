using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class AccessibleColumnModel : ColumnModel, IAccessibleColumnModel
{
  private readonly AccessibleMemberComponent _memberComponent;

  public AccessibleColumnModel(AccessibleMemberComponent memberComponent, short index)
    : base(memberComponent, index)
  {
    _memberComponent = memberComponent;
  }

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
