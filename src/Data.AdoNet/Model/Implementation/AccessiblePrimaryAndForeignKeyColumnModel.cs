using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class AccessiblePrimaryAndForeignKeyColumnModel : PrimaryAndForeignKeyColumnModel
{
  private readonly AccessibleMemberComponent _memberComponent;

  public AccessiblePrimaryAndForeignKeyColumnModel(AccessibleMemberComponent memberComponent, short index, short primaryKeyIndex, short foreignKeyIndex)
    : base(memberComponent, index, primaryKeyIndex, foreignKeyIndex)
  {
    _memberComponent = memberComponent;
  }

  [AllowNull]
  public override string Name
  {
    get => _memberComponent.Name;
    set => _memberComponent.Name = value;
  }
}
