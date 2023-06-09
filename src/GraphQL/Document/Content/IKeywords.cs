namespace CodeArchitects.Platform.GraphQL.Document.Content;

internal interface IKeywords<TSymbol>
{
  TSymbol Query { get; }
  TSymbol Mutation { get; }
  TSymbol Subscription { get; }
  TSymbol Null { get; }
  TSymbol True { get; }
  TSymbol False { get; }
}
