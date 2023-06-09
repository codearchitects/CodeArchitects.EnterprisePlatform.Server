namespace CodeArchitects.Platform.GraphQL.Document.Content;

internal interface IDocumentContentBuilder<TSymbol>
{
  public delegate void AppendAction<T>(IDocumentContentBuilder<TSymbol> builder, T current);
  public delegate void StatefulAppendAction<T>(IDocumentContentBuilder<TSymbol> builder, ref OperationAppender<TSymbol> caller, T current);

  int Length { get; }
  
  IKeywords<TSymbol> Keywords { get; }
  IPunctuators<TSymbol> Punctuators { get; }
  ITrivias<TSymbol> Trivias { get; }

  GraphDocument<TResult, TVariables> CreateDocument<TResult, TVariables>(OperationType operationType, string? name)
    where TVariables : notnull;

  IDocumentContentBuilder<TSymbol> Append(string @string);
  IDocumentContentBuilder<TSymbol> AppendCamelized(string @string);
  IDocumentContentBuilder<TSymbol> Append(TSymbol symbol);
  IDocumentContentBuilder<TSymbol> Append(TSymbol symbol, int repeatCount);

  IDocumentContentBuilder<TSymbol> AppendLiteral(float number);
  IDocumentContentBuilder<TSymbol> AppendLiteral(int number);
  IDocumentContentBuilder<TSymbol> AppendLiteral(string @string);

  IDocumentContentBuilder<TSymbol> AppendLine();

  void Pop();
}
