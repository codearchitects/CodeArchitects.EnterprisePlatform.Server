namespace CodeArchitects.Platform.GraphQL.Document.Builder.Content;

internal interface IContentBuilder<TSymbol>
{
  public delegate void AppendAction<T>(IContentBuilder<TSymbol> builder, T current);
  public delegate void StatefulAppendAction<T>(IContentBuilder<TSymbol> builder, ref DocumentContentBuilder<TSymbol> caller, T current);

  int Length { get; }
  
  IKeywords<TSymbol> Keywords { get; }
  IPunctuators<TSymbol> Punctuators { get; }
  ITrivias<TSymbol> Trivias { get; }

  GraphDocument<TResult> GetDocument<TResult>();

  GraphDocument<TResult, TVariables> GetDocument<TResult, TVariables>()
    where TVariables : notnull;

  IContentBuilder<TSymbol> Append(string s);
  IContentBuilder<TSymbol> Append(ReadOnlySpan<char> s);
  IContentBuilder<TSymbol> Append(TSymbol symbol);
  IContentBuilder<TSymbol> Append(TSymbol symbol, int repeatCount);
  IContentBuilder<TSymbol> Append(OperationType operationType);

  IContentBuilder<TSymbol> AppendCamelized(string s);
  IContentBuilder<TSymbol> AppendLiteral(double d);
  IContentBuilder<TSymbol> AppendLiteral(long l);
  IContentBuilder<TSymbol> AppendLiteral(string s);

  IContentBuilder<TSymbol> AppendLine();
  void Pop();
}
