namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class JoinKeyPair : IKeyPair
{
  private readonly JoinColumnModel _joinColumn;
  private readonly bool _isFrom;

  public JoinKeyPair(IPrimaryKeyColumnModel primaryKeyColumn, JoinColumnModel joinColumn, bool isFrom)
  {
    PrimaryKeyColumn = primaryKeyColumn;
    _joinColumn = joinColumn;
    _isFrom = isFrom;
  }

  public IPrimaryKeyColumnModel PrimaryKeyColumn { get; }

  public IColumnModel ForeignKeyColumn => _joinColumn;

  public IColumnModel FromColumn => _isFrom ? PrimaryKeyColumn : _joinColumn;

  public IColumnModel ToColumn => _isFrom ? _joinColumn : PrimaryKeyColumn;
}
