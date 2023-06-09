using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

internal interface IDocumentBuilder<TDocumentRoot>
  where TDocumentRoot : notnull
{
  GraphDocument<TField> Query<TField>(Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TField>>> expansion);

  GraphDocument<TField, TVariables> Query<TField, TVariables>(Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TField, TVariables>>> expansion)
    where TVariables : notnull;

  GraphDocument<TField> Query<TField>(string name, Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TField>>> expansion);

  GraphDocument<TField, TVariables> Query<TField, TVariables>(string name, Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TField, TVariables>>> expansion)
    where TVariables : notnull;


  GraphDocument<TField> Mutation<TField>(Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TField>>> expansion);

  GraphDocument<TField, TVariables> Mutation<TField, TVariables>(Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TField, TVariables>>> expansion)
    where TVariables : notnull;

  GraphDocument<TField> Mutation<TField>(string name, Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TField>>> expansion);

  GraphDocument<TField, TVariables> Mutation<TField, TVariables>(string name, Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TField, TVariables>>> expansion)
    where TVariables : notnull;
}
