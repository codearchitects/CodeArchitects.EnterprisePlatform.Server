using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

public interface IFieldBuilder<TField> : IBuildResult<TField>, IBuilder
{
  IFieldBuilder<TField> WithArgument<TArg>(string name, TArg value);

  IFieldBuilderWithArguments<TField> WithDirective(string name);

  IFieldBuilderWithArguments<TField> WithDirective(string name, Expression<Func<IDirectiveBuilder, IDirectiveBuilder>> directive);

  IBuildResult<TResult> WithSelection<TResult>(Expression<Func<TField, TResult>> selection);

  IBuildResult<TResult> WithSelectionSet<TResult>(Expression<Func<ISelectionSetBuilder<TField>, IBuildResult<TResult>>> selection);
}

public interface IFieldBuilderWithArguments<TField> : IBuildResult<TField>, IBuilder
{
  IFieldBuilderWithArguments<TField> WithDirective(string name);

  IFieldBuilderWithArguments<TField> WithDirective(string name, Expression<Func<IDirectiveBuilder, IDirectiveBuilder>> directive);

  IBuildResult<TResult> WithSelection<TResult>(Expression<Func<TField, TResult>> selection);

  IBuildResult<TResult> WithSelectionSet<TResult>(Expression<Func<ISelectionSetBuilder<TField>, IBuildResult<TResult>>> selection);
}

public interface IFieldBuilder<TField, TVariables> : IBuildResult<TField, TVariables>, IBuilder<TVariables>
  where TVariables : notnull
{
  IFieldBuilder<TField, TVariables> WithArgument<TArg>(string name, TArg value);

  IFieldBuilder<TField, TVariables> WithArgument<TArg>(string name, Expression<Func<TVariables, TArg>> variable);

  IFieldBuilder<TField, TVariables> WithArgument<TArg>(Expression<Func<TVariables, TArg>> variable);

  IFieldBuilderWithArguments<TField, TVariables> WithDirective(string name);

  IFieldBuilderWithArguments<TField, TVariables> WithDirective(string name, Expression<Func<IDirectiveBuilder<TVariables>, IDirectiveBuilder<TVariables>>> directive);

  IBuildResult<TResult, TVariables> WithSelection<TResult>(Expression<Func<TField, TResult>> selection);

  IBuildResult<TResult, TVariables> WithSelectionSet<TResult>(Expression<Func<ISelectionSetBuilder<TField, TVariables>, IBuildResult<TResult, TVariables>>> selection);
}

public interface IFieldBuilderWithArguments<TField, TVariables> : IBuildResult<TField, TVariables>, IBuilder<TVariables>
  where TVariables : notnull
{
  IFieldBuilderWithArguments<TField, TVariables> WithDirective(string name);

  IFieldBuilderWithArguments<TField, TVariables> WithDirective(string name, Expression<Func<IDirectiveBuilder<TVariables>, IDirectiveBuilder<TVariables>>> directive);

  IBuildResult<TResult, TVariables> WithSelection<TResult>(Expression<Func<TField, TResult>> selection);

  IBuildResult<TResult, TVariables> WithSelectionSet<TResult>(Expression<Func<ISelectionSetBuilder<TField, TVariables>, IBuildResult<TResult, TVariables>>> selection);
}
