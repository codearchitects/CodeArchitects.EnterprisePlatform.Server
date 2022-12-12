namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class HiddenForeignKeyColumnModel : ForeignKeyColumnModel
{
  private readonly HiddenMemberComponent<object?> _memberComponent;

  public HiddenForeignKeyColumnModel(HiddenMemberComponent<object?> memberComponent, short index, short foreignKeyIndex, ISimpleNavigationModel navigation, string name)
    : base(index, foreignKeyIndex, navigation)
  {
    _memberComponent = memberComponent;
    Name = name;
  }

  protected override MemberComponent<object?> MemberComponent => _memberComponent;

  public override string Name { get; set; }
}
