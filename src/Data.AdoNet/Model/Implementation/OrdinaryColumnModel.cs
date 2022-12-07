namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class OrdinaryColumnModel : AccessibleColumnModel, IOrdinaryColumnModel
{
  public OrdinaryColumnModel(AccessibleMemberComponent memberComponent, short index)
    : base(memberComponent, index)
  {
  }

  public override bool IsPrimaryKey => false;

  public override bool IsForeignKey => false;

  public override TResult Accept<TVisitor, TResult>(in TVisitor visitor)
  {
    return visitor.VisitOrdinary(this);
  }

  public override TResult Accept<TVisitor, TResult, TState>(in TVisitor visitor, in TState state)
  {
    return visitor.VisitOrdinary(this, in state);
  }
}
