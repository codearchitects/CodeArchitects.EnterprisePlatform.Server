namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class HiddenForeignKeyColumnModel : ForeignKeyColumnModel
{
  private readonly HiddenMemberComponent<object?> _memberComponent;

  public HiddenForeignKeyColumnModel(HiddenMemberComponent<object?> memberComponent, short index)
    : base(index)
  {
    _memberComponent = memberComponent;
  }

  protected override MemberComponent<object?> MemberComponent => _memberComponent;

  public override string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
