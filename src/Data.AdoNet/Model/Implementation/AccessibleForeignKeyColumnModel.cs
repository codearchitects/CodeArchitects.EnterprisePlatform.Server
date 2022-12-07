using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class AccessibleForeignKeyColumnModel : ForeignKeyColumnModel
{
  private readonly AccessibleMemberComponent _memberComponent;

  public AccessibleForeignKeyColumnModel(AccessibleMemberComponent memberComponent, short index, short foreignKeyIndex)
    : base(memberComponent, index, foreignKeyIndex)
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
