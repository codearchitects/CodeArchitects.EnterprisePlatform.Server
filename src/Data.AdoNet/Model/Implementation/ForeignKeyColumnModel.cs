namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class ForeignKeyColumnModel : ColumnModel, IForeignKeyColumnModel
{
  public ForeignKeyColumnModel(short index, short foreignKeyIndex, INavigationModel navigation)
    : base(index)
  {
    ForeignKeyIndex = foreignKeyIndex;
    Navigation = navigation;
  }

  public override bool IsPrimaryKey => false;

  public override bool IsForeignKey => true;

  public short ForeignKeyIndex { get; }

  public IPrimaryKeyColumnModel PrimaryKeyColumn => Navigation.PrimaryKey.Columns[ForeignKeyIndex];

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
