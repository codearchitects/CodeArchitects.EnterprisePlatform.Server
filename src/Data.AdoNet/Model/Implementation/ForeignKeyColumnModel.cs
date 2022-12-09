namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class ForeignKeyColumnModel : ColumnModel, IForeignKeyColumnModel
{
  public ForeignKeyColumnModel(short index, INavigationModel navigation)
    : base(index)
  {
    Navigation = navigation;
  }

  public override bool IsPrimaryKey => false;

  public override bool IsForeignKey => true;

  public short ForeignKeyIndex => PrimaryKeyColumn.PrimaryKeyIndex;

  public IPrimaryKeyColumnModel PrimaryKeyColumn => throw new NotImplementedException();

  public INavigationModel Navigation { get; }

  public override TResult Accept<TVisitor, TResult>(in TVisitor visitor)
  {
    return visitor.VisitForeignKey(this);
  }

  public override TResult Accept<TVisitor, TResult, TState>(in TVisitor visitor, in TState state)
  {
    return visitor.VisitForeignKey(this, in state);
  }
}
