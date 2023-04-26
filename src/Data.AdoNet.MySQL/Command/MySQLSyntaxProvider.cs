using CodeArchitects.Platform.Data.AdoNet.Command;

namespace CodeArchitects.Platform.Data.AdoNet.MySQL.Command;

internal class MySQLSyntaxProvider : ISyntaxProvider
{
  public char EscapeLeft => '`';

  public char EscapeRight => '`';

  public char ParameterPrefix => '@';

  public bool HasOutputBefore => false;

  public bool HasOutputAfter => true;

  public bool AppendASKeyword => true;

  public string GetOutputAfter(string tableName, string columnName)
  {
    return $"RETURNING `{tableName}`.`{columnName}`";
  }

  public string GetOutputBefore(string tableName, string columnName)
  {
    throw new NotSupportedException();
  }
}
