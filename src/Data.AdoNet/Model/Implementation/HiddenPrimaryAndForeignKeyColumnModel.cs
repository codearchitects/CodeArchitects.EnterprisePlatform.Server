namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class HiddenPrimaryAndForeignKeyColumnModel : PrimaryAndForeignKeyColumnModel
{
  public HiddenPrimaryAndForeignKeyColumnModel(MemberComponent memberComponent, short index, short primaryKeyIndex, short foreignKeyIndex)
    : base(memberComponent, index, primaryKeyIndex, foreignKeyIndex)
  {
  }

  public override string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
