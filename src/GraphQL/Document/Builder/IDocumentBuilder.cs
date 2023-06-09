using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

internal interface IDocumentBuilder<TDocumentRoot>
  where TDocumentRoot : notnull
{
  IGraphDocument<TResult> Query<TResult>(Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion);

  IGraphDocument<TResult, TVariables> Query<TResult, TVariables>(Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull;

  IGraphDocument<TResult> Query<TResult>(string name, Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion);

  IGraphDocument<TResult, TVariables> Query<TResult, TVariables>(string name, Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull;


  IGraphDocument<TResult> Mutation<TResult>(Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion);

  IGraphDocument<TResult, TVariables> Mutation<TResult, TVariables>(Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull;

  IGraphDocument<TResult> Mutation<TResult>(string name, Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion);

  IGraphDocument<TResult, TVariables> Mutation<TResult, TVariables>(string name, Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull;
}
