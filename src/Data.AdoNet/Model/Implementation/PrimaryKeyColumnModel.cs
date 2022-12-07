namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class PrimaryKeyColumnModel : AccessibleColumnModel, IPrimaryKeyColumnModel
{
  public PrimaryKeyColumnModel(AccessibleMemberComponent memberComponent, short index, short primaryKeyIndex)
    : base(memberComponent, index)
  {
    PrimaryKeyIndex = primaryKeyIndex;
  }

  public override bool IsPrimaryKey => true;

  public override bool IsForeignKey => false;

  public short PrimaryKeyIndex { get; }

  public override TResult Accept<TVisitor, TResult>(in TVisitor visitor)
  {
    return visitor.VisitPrimaryKey(this);
  }

  public override TResult Accept<TVisitor, TResult, TState>(in TVisitor visitor, in TState state)
  {
    return visitor.VisitPrimaryKey(this, in state);
  }
}
