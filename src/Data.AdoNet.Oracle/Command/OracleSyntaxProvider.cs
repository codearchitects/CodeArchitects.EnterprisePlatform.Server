using CodeArchitects.Platform.Data.AdoNet.Command;

namespace CodeArchitects.Platform.Data.AdoNet.Oracle.Command;

internal class OracleSyntaxProvider : ISyntaxProvider
{
  public char EscapeLeft => '"';

  public char EscapeRight => '"';

  public char ParameterPrefix => ':';

  public bool HasOutputBefore => false;

  public bool HasOutputAfter => true;

  public bool AppendASKeyword => false;

  public string GetOutputAfter(string tableName, string columnName)
  {
    return $"RETURNING \"{columnName}\" INTO \"{columnName}\"";
  }

  public string GetOutputBefore(string tableName, string columnName)
  {
    throw new NotSupportedException();
  }
}
