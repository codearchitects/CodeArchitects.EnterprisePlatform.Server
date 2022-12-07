namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class ForeignKeyColumnModel : ColumnModel, IForeignKeyColumnModel
{
  public ForeignKeyColumnModel(short index, short foreignKeyIndex)
    : base(index)
  {
    ForeignKeyIndex = foreignKeyIndex;
  }

  public override bool IsPrimaryKey => false;

  public override bool IsForeignKey => true;

  public short ForeignKeyIndex { get; }

  public IPrimaryKeyColumnModel PrimaryKeyColumn => throw new NotImplementedException();

  public NavigationModel Navigation => throw new NotImplementedException();

  public override TResult Accept<TVisitor, TResult>(in TVisitor visitor)
  {
    return visitor.VisitForeignKey(this);
  }

  public override TResult Accept<TVisitor, TResult, TState>(in TVisitor visitor, in TState state)
  {
    return visitor.VisitForeignKey(this, in state);
  }


  #region Explicit implementation

  INavigationModel IForeignKeyColumnModelBase.Navigation => throw new NotImplementedException();

  #endregion
}
