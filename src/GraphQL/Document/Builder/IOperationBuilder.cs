using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

internal interface IOperationBuilder<TField> : IExpander, ISelector
{
  IOperationBuilder<TField> WithDirective(string name);

  IOperationBuilder<TField> WithDirective(string name, Expression<Func<IDirectiveBuilder, IDirectiveBuilder>> directive);

  IBuildResult<TResult> WithSelection<TResult>(Expression<Func<TField, TResult>> selection);
}

internal interface IOperationBuilder<TField, TVariables> : IExpander<TVariables>, ISelector
  where TVariables : notnull
{
  IOperationBuilder<TField, TVariables> WithDirective(string name);

  IOperationBuilder<TField, TVariables> WithDirective(string name, Expression<Func<IDirectiveBuilder<TVariables>, IDirectiveBuilder<TVariables>>> directive);

  IBuildResult<TResult, TVariables> WithSelection<TResult>(Expression<Func<TField, TResult>> selection);
}
