namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class ForeignKeyColumnModel : ColumnModel, IForeignKeyColumnModel
{
  private readonly ISimpleNavigationModel _navigation;

  public ForeignKeyColumnModel(short index, short foreignKeyIndex, ISimpleNavigationModel navigation)
    : base(index)
  {
    ForeignKeyIndex = foreignKeyIndex;
    _navigation = navigation;
  }

  public override bool IsPrimaryKey => false;

  public override bool IsForeignKey => true;

  public override bool IsConcurrencyToken
  {
    get => false;
    set => throw new ModelConfigurationException("Primary keys and foreign keys cannot be used as concurrency tokens.");
  }

  public short ForeignKeyIndex { get; }

  public IPrimaryKeyColumnModel PrimaryKeyColumn => _navigation.PrimaryKey.Columns[ForeignKeyIndex];

  public INavigationModel Navigation => _navigation;

  public override TResult Accept<TResult>(IColumnModelVisitor<TResult> visitor)
  {
    return visitor.VisitForeignKey(this);
  }

  public override TResult Accept<TResult, TState>(IColumnModelVisitor<TResult, TState> visitor, in TState state)
  {
    return visitor.VisitForeignKey(this, in state);
  }
}
