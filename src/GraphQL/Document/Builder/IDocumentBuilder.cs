using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

public interface IDocumentBuilder<TDocumentRoot>
  where TDocumentRoot : notnull
{
  GraphDocument<TResult> Query<TResult>(Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion)
    where TResult : class;

  GraphDocument<TResult, TVariables> Query<TResult, TVariables>(Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TResult : class
    where TVariables : notnull;

  GraphDocument<TResult> Query<TResult>(string name, Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion)
    where TResult : class;

  GraphDocument<TResult, TVariables> Query<TResult, TVariables>(string name, Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TResult : class
    where TVariables : notnull;


  GraphDocument<TResult> Mutation<TResult>(Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion)
    where TResult : class;

  GraphDocument<TResult, TVariables> Mutation<TResult, TVariables>(Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TResult : class
    where TVariables : notnull;

  GraphDocument<TResult> Mutation<TResult>(string name, Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion)
    where TResult : class;

  GraphDocument<TResult, TVariables> Mutation<TResult, TVariables>(string name, Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TResult : class
    where TVariables : notnull;


  GraphDocument<TResult> Raw<TResult>(string document)
    where TResult : class;

  GraphDocument<TResult, TVariables> Raw<TResult, TVariables>(string document)
    where TResult : class
    where TVariables : notnull;
}
