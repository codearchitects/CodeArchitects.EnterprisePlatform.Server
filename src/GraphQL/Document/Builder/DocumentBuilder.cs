using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

internal class DocumentBuilder<TDocumentRoot> : IDocumentBuilder<TDocumentRoot>
  where TDocumentRoot : class
{
  public static readonly DocumentBuilder<TDocumentRoot> Instance = new();

  private DocumentBuilder() { }

  public GraphDocument<TResult> Query<TResult>(Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion)
  {
    return new GraphDocument<TResult>.Query(null, expansion.Body);
  }

  public GraphDocument<TResult, TVariables> Query<TResult, TVariables>(Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull
  {
    return new GraphDocument<TResult, TVariables>.Query(null, expansion.Body);
  }

  public GraphDocument<TResult> Query<TResult>(string name, Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion)
  {
    return new GraphDocument<TResult>.Query(name, expansion.Body);
  }

  public GraphDocument<TResult, TVariables> Query<TResult, TVariables>(string name, Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull
  {
    return new GraphDocument<TResult, TVariables>.Query(name, expansion.Body);
  }

  public GraphDocument<TResult> Mutation<TResult>(Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion)
  {
    return new GraphDocument<TResult>.Mutation(null, expansion.Body);
  }

  public GraphDocument<TResult, TVariables> Mutation<TResult, TVariables>(Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull
  {
    return new GraphDocument<TResult, TVariables>.Mutation(null, expansion.Body);
  }

  public GraphDocument<TResult> Mutation<TResult>(string name, Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion)
  {
    return new GraphDocument<TResult>.Mutation(name, expansion.Body);
  }

  public GraphDocument<TResult, TVariables> Mutation<TResult, TVariables>(string name, Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull
  {
    return new GraphDocument<TResult, TVariables>.Mutation(name, expansion.Body);
  }
}
