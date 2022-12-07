namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class HiddenForeignKeyColumnModel : ForeignKeyColumnModel
{
  private readonly HiddenMemberComponent _memberComponent;

  public HiddenForeignKeyColumnModel(HiddenMemberComponent memberComponent, short index)
    : base(index)
  {
    _memberComponent = memberComponent;
  }

  protected override MemberComponent MemberComponent => _memberComponent;

  public override string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
