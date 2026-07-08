namespace CodeArchitects.Platform.Data.AdoNet.Command;

internal interface ISyntaxProvider
{
  char EscapeLeft { get; }
  char EscapeRight { get; }
  char ParameterPrefix { get; }
  bool AppendASKeyword { get; }
  bool HasOutputBefore { get; }
  bool HasOutputAfter { get; }

  string GetOutputAfter(string tableName, string columnName);
  string GetOutputBefore(string tableName, string columnName);
}
