namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class PrimaryAndForeignKeyColumnModel : ColumnModel, IPrimaryAndForeignKeyColumnModel
{
  public PrimaryAndForeignKeyColumnModel(short index, short primaryKeyIndex, short foreignKeyIndex)
    : base(index)
  {
    PrimaryKeyIndex = primaryKeyIndex;
    ForeignKeyIndex = foreignKeyIndex;
  }

  public override bool IsPrimaryKey => true;

  public override bool IsForeignKey => true;

  public short PrimaryKeyIndex { get; }

  public short ForeignKeyIndex { get; }

  public IPrimaryKeyColumnModel PrimaryKeyColumn => throw new NotImplementedException();

  public NavigationModel Navigation => throw new NotImplementedException();

  public override TResult Accept<TVisitor, TResult>(in TVisitor visitor)
  {
    return visitor.VisitPrimaryAndForeignKey(this);
  }

  public override TResult Accept<TVisitor, TResult, TState>(in TVisitor visitor, in TState state)
  {
    return visitor.VisitPrimaryAndForeignKey(this, in state);
  }

  #region Explicit implementation

  INavigationModel IForeignKeyColumnModelBase.Navigation => Navigation;

  #endregion
}
