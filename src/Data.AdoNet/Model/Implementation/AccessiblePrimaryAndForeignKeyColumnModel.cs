using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class AccessiblePrimaryAndForeignKeyColumnModel : PrimaryAndForeignKeyColumnModel
{
  private readonly AccessibleMemberComponent _memberComponent;

  public AccessiblePrimaryAndForeignKeyColumnModel(AccessibleMemberComponent memberComponent, short index, short primaryKeyIndex, short foreignKeyIndex)
    : base(index, primaryKeyIndex, foreignKeyIndex)
  {
    _memberComponent = memberComponent;
  }

  protected override MemberComponent MemberComponent => _memberComponent;

  [AllowNull]
  public override string Name
  {
    get => _memberComponent.Name;
    set => _memberComponent.Name = value;
  }
}
