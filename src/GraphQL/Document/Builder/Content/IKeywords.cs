namespace CodeArchitects.Platform.GraphQL.Document.Builder.Content;

internal interface IKeywords<TSymbol>
{
  TSymbol Null { get; }
  TSymbol True { get; }
  TSymbol False { get; }
}
