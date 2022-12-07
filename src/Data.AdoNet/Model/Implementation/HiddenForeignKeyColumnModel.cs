namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class HiddenForeignKeyColumnModel : ForeignKeyColumnModel
{
  public HiddenForeignKeyColumnModel(HiddenMemberComponent memberComponent, short index, short foreignKeyIndex)
    : base(memberComponent, index, foreignKeyIndex)
  {
  }

  public override string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
