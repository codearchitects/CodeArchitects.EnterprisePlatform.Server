namespace CodeArchitects.Platform.GraphQL.Document.Content;

internal interface IPunctuators<TSymbol>
{
  TSymbol Bang { get; }
  TSymbol Colon { get; }
  TSymbol DollarSign { get; }
  TSymbol AtSign { get; }
  TSymbol LeftParenthesis { get; }
  TSymbol RightParenthesis { get; }
  TSymbol LeftBracket { get; }
  TSymbol RightBracket { get; }
  TSymbol LeftBrace { get; }
  TSymbol RightBrace { get; }
}
