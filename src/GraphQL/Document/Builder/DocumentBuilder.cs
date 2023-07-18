using CodeArchitects.Platform.GraphQL.Document.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Literal;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

internal class DocumentBuilder<TDocumentRoot> : IDocumentBuilder<TDocumentRoot>
  where TDocumentRoot : class
{
  public static readonly DocumentBuilder<TDocumentRoot> Instance = new();

  private DocumentBuilder() { }

  public GraphDocument<TResult> Query<TResult>(Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion)
    where TResult : class
  {
    return new ExpressionGraphDocument<TResult>.Query(string.Empty, expansion.Body);
  }

  public GraphDocument<TResult, TVariables> Query<TResult, TVariables>(Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TResult : class
    where TVariables : notnull
  {
    return new ExpressionGraphDocument<TResult, TVariables>.Query(string.Empty, expansion.Body);
  }

  public GraphDocument<TResult> Query<TResult>(string name, Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion)
    where TResult : class
  {
    return new ExpressionGraphDocument<TResult>.Query(name, expansion.Body);
  }

  public GraphDocument<TResult, TVariables> Query<TResult, TVariables>(string name, Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TResult : class
    where TVariables : notnull
  {
    return new ExpressionGraphDocument<TResult, TVariables>.Query(name, expansion.Body);
  }

  public GraphDocument<TResult> Mutation<TResult>(Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion)
    where TResult : class
  {
    return new ExpressionGraphDocument<TResult>.Mutation(string.Empty, expansion.Body);
  }

  public GraphDocument<TResult, TVariables> Mutation<TResult, TVariables>(Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TResult : class
    where TVariables : notnull
  {
    return new ExpressionGraphDocument<TResult, TVariables>.Mutation(string.Empty, expansion.Body);
  }

  public GraphDocument<TResult> Mutation<TResult>(string name, Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion)
    where TResult : class
  {
    return new ExpressionGraphDocument<TResult>.Mutation(name, expansion.Body);
  }

  public GraphDocument<TResult, TVariables> Mutation<TResult, TVariables>(string name, Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TResult : class
    where TVariables : notnull
  {
    return new ExpressionGraphDocument<TResult, TVariables>.Mutation(name, expansion.Body);
  }


  public GraphDocument<TResult> Raw<TResult>(string document)
    where TResult : class
  {
    return new LiteralGraphDocument<TResult>(document);
  }

  public GraphDocument<TResult, TVariables> Raw<TResult, TVariables>(string document)
    where TResult : class
    where TVariables : notnull
  {
    return new LiteralGraphDocument<TResult, TVariables>(document);
  }
}
