namespace CodeArchitects.Platform.GraphQL.Document.Content;

internal interface ITrivias<TSymbol>
{
  TSymbol Space { get; }
  TSymbol Comma { get; }
}
