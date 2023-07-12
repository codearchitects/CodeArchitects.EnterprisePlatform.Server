using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

internal class DocumentBuilder<TDocumentRoot> : IDocumentBuilder<TDocumentRoot>
  where TDocumentRoot : class
{
  public static readonly DocumentBuilder<TDocumentRoot> Instance = new();

  private DocumentBuilder()
  {
  }

  public GraphDocument<TResult> Query<TResult>(Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion)
  {
    return new GraphQuery<TResult>(null, expansion.Body);
  }

  public GraphDocument<TResult, TVariables> Query<TResult, TVariables>(Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull
  {
    return new GraphQuery<TResult, TVariables>(null, expansion.Body);
  }

  public GraphDocument<TResult> Query<TResult>(string name, Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion)
  {
    return new GraphQuery<TResult>(name, expansion.Body);
  }

  public GraphDocument<TResult, TVariables> Query<TResult, TVariables>(string name, Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull
  {
    return new GraphQuery<TResult, TVariables>(name, expansion.Body);
  }

  public GraphDocument<TResult> Mutation<TResult>(Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion)
  {
    return new GraphMutation<TResult>(null, expansion.Body);
  }

  public GraphDocument<TResult, TVariables> Mutation<TResult, TVariables>(Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull
  {
    return new GraphMutation<TResult, TVariables>(null, expansion.Body);
  }

  public GraphDocument<TResult> Mutation<TResult>(string name, Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion)
  {
    return new GraphMutation<TResult>(name, expansion.Body);
  }

  public GraphDocument<TResult, TVariables> Mutation<TResult, TVariables>(string name, Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull
  {
    return new GraphMutation<TResult, TVariables>(name, expansion.Body);
  }
}
