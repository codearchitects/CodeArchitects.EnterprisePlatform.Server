namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class HiddenPrimaryAndForeignKeyColumnModel : PrimaryAndForeignKeyColumnModel
{
  private readonly MemberComponent _memberComponent;

  public HiddenPrimaryAndForeignKeyColumnModel(MemberComponent memberComponent, short index, short primaryKeyIndex, short foreignKeyIndex)
    : base(index, primaryKeyIndex, foreignKeyIndex)
  {
    _memberComponent = memberComponent;
  }

  protected override MemberComponent MemberComponent => _memberComponent;

  public override string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
