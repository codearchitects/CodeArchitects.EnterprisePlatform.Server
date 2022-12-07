namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class PrimaryAndForeignKeyColumnModel : ColumnModel, IPrimaryAndForeignKeyColumnModel
{
  public PrimaryAndForeignKeyColumnModel(MemberComponent memberComponent, short index)
    : base(memberComponent, index)
  {
  }

  public override bool IsPrimaryKey => throw new NotImplementedException();

  public override bool IsForeignKey => throw new NotImplementedException();

  public override string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

  public short PrimaryKeyIndex => throw new NotImplementedException();

  public short ForeignKeyIndex => throw new NotImplementedException();

  public IPrimaryKeyColumnModel PrimaryKeyColumn => throw new NotImplementedException();

  public INavigationModel Navigation => throw new NotImplementedException();

  public override TResult Accept<TVisitor, TResult>(in TVisitor visitor)
  {
    return visitor.VisitPrimaryAndForeignKey(this);
  }

  public override TResult Accept<TVisitor, TResult, TState>(in TVisitor visitor, in TState state)
  {
    return visitor.VisitPrimaryAndForeignKey(this, in state);
  }
}
