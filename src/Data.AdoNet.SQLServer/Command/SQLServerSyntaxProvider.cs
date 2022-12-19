using CodeArchitects.Platform.Data.AdoNet.Command;

namespace CodeArchitects.Platform.Data.AdoNet.SQLServer.Command;

internal class SQLServerSyntaxProvider : ISyntaxProvider
{
  public char EscapeLeft => '[';

  public char EscapeRight => ']';

  public char ParameterPrefix => '@';

  public bool HasOutputBefore => true;

  public bool HasOutputAfter => false;

  public string GetOutputAfter(string tableName, string columnName)
  {
    throw new NotSupportedException();
  }

  public string GetOutputBefore(string tableName, string columnName)
  {
    return $"OUTPUT INSERTED.[{columnName}]";
  }
}
