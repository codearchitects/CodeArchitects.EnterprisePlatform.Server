using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

public interface IOperationBuilder<TField> : IBuilder
{
  IOperationBuilder<TField> WithDirective(string name);

  IOperationBuilder<TField> WithDirective(string name, Expression<Func<IDirectiveBuilder, IDirectiveBuilder>> directive);

  IBuildResult<TResult> WithSelection<TResult>(Expression<Func<TField, TResult>> selection);

  IBuildResult<TResult> WithSelectionSet<TResult>(Expression<Func<ISelectionSetBuilder<TField>, IBuildResult<TResult>>> selection);
}

public interface IOperationBuilder<TField, TVariables> : IBuilder<TVariables>
  where TVariables : notnull
{
  IOperationBuilder<TField, TVariables> WithDirective(string name);

  IOperationBuilder<TField, TVariables> WithDirective(string name, Expression<Func<IDirectiveBuilder<TVariables>, IDirectiveBuilder<TVariables>>> directive);

  IBuildResult<TResult, TVariables> WithSelection<TResult>(Expression<Func<TField, TResult>> selection);

  IBuildResult<TResult, TVariables> WithSelectionSet<TResult>(Expression<Func<ISelectionSetBuilder<TField, TVariables>, IBuildResult<TResult, TVariables>>> selection);
}
