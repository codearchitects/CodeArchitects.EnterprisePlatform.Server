namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class KeyPair : IKeyPair
{
  private readonly IForeignKeyColumnModel _foreignKeyColumn;
  private readonly bool _isOnDependent;

  public KeyPair(IForeignKeyColumnModel foreignKeyColumn, bool isOnDependent)
  {
    _foreignKeyColumn = foreignKeyColumn;
    _isOnDependent = isOnDependent;
  }

  public IPrimaryKeyColumnModel PrimaryKeyColumn => _foreignKeyColumn.PrimaryKeyColumn;

  public IForeignKeyColumnModel ForeignKeyColumn => _foreignKeyColumn;

  public IColumnModel FromColumn => _isOnDependent ? ForeignKeyColumn : PrimaryKeyColumn;

  public IColumnModel ToColumn => _isOnDependent ? PrimaryKeyColumn : ForeignKeyColumn;
}
