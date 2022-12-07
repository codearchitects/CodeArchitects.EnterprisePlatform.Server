using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class AccessibleForeignKeyColumnModel : ForeignKeyColumnModel
{
  private readonly AccessibleMemberComponent<object?> _memberComponent;

  public AccessibleForeignKeyColumnModel(AccessibleMemberComponent<object?> memberComponent, short index)
    : base(index)
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
}
