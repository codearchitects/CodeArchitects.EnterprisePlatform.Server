using CodeArchitects.Platform.GraphQL.Document;
using CodeArchitects.Platform.GraphQL.Document.Builder;

namespace CodeArchitects.Platform.GraphQL;

public static partial class GQL
{
  public static class Builder
  {
    public static GraphDocument<TResult> Compile<TResult>(Func<IDocumentBuilder<IDocumentRoot>, GraphDocument<TResult>> buildDocument)
      where TResult : class
    {
      if (buildDocument is null)
        throw new ArgumentNullException(nameof(buildDocument));

      return Builder<IDocumentRoot>.CompileCore(buildDocument);
    }

    public static GraphDocument<TResult, TVariables> Compile<TResult, TVariables>(Func<IDocumentBuilder<IDocumentRoot>, GraphDocument<TResult, TVariables>> buildDocument)
      where TResult : class
      where TVariables : class
    {
      if (buildDocument is null)
        throw new ArgumentNullException(nameof(buildDocument));

      return Builder<IDocumentRoot>.CompileCore(buildDocument);
    }
  }

  public static class Builder<TDocumentRoot>
    where TDocumentRoot : class
  {
    public static GraphDocument<TResult> Compile<TResult>(Func<IDocumentBuilder<TDocumentRoot>, GraphDocument<TResult>> buildDocument)
      where TResult : class
    {
      if (buildDocument is null)
        throw new ArgumentNullException(nameof(buildDocument));

      return CompileCore(buildDocument);
    }

    public static GraphDocument<TResult, TVariables> Compile<TResult, TVariables>(Func<IDocumentBuilder<TDocumentRoot>, GraphDocument<TResult, TVariables>> buildDocument)
      where TResult : class
      where TVariables : class
    {
      if (buildDocument is null)
        throw new ArgumentNullException(nameof(buildDocument));

      return CompileCore(buildDocument);
    }

    internal static GraphDocument<TResult> CompileCore<TResult>(Func<IDocumentBuilder<TDocumentRoot>, GraphDocument<TResult>> buildDocument)
      where TResult : class
    {
      return buildDocument(DocumentBuilder<TDocumentRoot>.Instance);
    }

    internal static GraphDocument<TResult, TVariables> CompileCore<TResult, TVariables>(Func<IDocumentBuilder<TDocumentRoot>, GraphDocument<TResult, TVariables>> buildDocument)
      where TResult : class
      where TVariables : class
    {
      return buildDocument(DocumentBuilder<TDocumentRoot>.Instance);
    }
  }
}
