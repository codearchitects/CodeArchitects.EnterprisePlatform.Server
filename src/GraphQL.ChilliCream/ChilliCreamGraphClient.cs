using CodeArchitects.Platform.GraphQL.Document;
using CodeArchitects.Platform.GraphQL.Document.Builder;

namespace CodeArchitects.Platform.GraphQL.ChilliCream;

internal class ChilliCreamGraphClient<TDocumentRoot> : GraphClient<TDocumentRoot>
  where TDocumentRoot : class
{
  public ChilliCreamGraphClient(IDocumentBuilder<TDocumentRoot> documentBuilder)
    : base(documentBuilder)
  {
  }

  protected override IGraphRequest<TResult> RequestCore<TResult>(IGraphDocument<TResult> document)
  {
    throw new NotImplementedException();
  }

  protected override IGraphRequest<TResult, TVariables> RequestCore<TResult, TVariables>(IGraphDocument<TResult, TVariables> document)
  {
    throw new NotImplementedException();
  }
}
