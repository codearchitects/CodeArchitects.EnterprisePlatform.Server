using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

internal interface IFieldBuilder<TField> : IBuildResult<TField>, IExpander, ISelector
{
  IFieldBuilder<TField> WithArgument<TArg>(string name, TArg value);

  IFieldBuilderWithArguments<TField> WithDirective(string name);

  IFieldBuilderWithArguments<TField> WithDirective(string name, Expression<Func<IDirectiveBuilder, IDirectiveBuilder>> directive);

  IBuildResult<TResult> WithSelection<TResult>(Expression<Func<TField, TResult>> selection);
}

internal interface IFieldBuilderWithArguments<TField> : IBuildResult<TField>, IExpander, ISelector
{
  IFieldBuilderWithArguments<TField> WithDirective(string name);

  IFieldBuilderWithArguments<TField> WithDirective(string name, Expression<Func<IDirectiveBuilder, IDirectiveBuilder>> directive);

  IBuildResult<TResult> WithSelection<TResult>(Expression<Func<TField, TResult>> selection);
}

internal interface IFieldBuilder<TField, TVariables> : IBuildResult<TField, TVariables>, IExpander<TVariables>, ISelector
  where TVariables : notnull
{
  IFieldBuilder<TField, TVariables> WithArgument<TArg>(string name, TArg value);

  IFieldBuilder<TField, TVariables> WithArgument<TArg>(string name, Expression<Func<TVariables, TArg>> variable);

  IFieldBuilder<TField, TVariables> WithArgument<TArg>(Expression<Func<TVariables, TArg>> variable);

  IFieldBuilderWithArguments<TField, TVariables> WithDirective(string name);

  IFieldBuilderWithArguments<TField, TVariables> WithDirective(string name, Expression<Func<IDirectiveBuilder<TVariables>, IDirectiveBuilder<TVariables>>> directive);

  IBuildResult<TResult, TVariables> WithSelection<TResult>(Expression<Func<TField, TResult>> selection);
}

internal interface IFieldBuilderWithArguments<TField, TVariables> : IBuildResult<TField, TVariables>, IExpander<TVariables>, ISelector
  where TVariables : notnull
{
  IFieldBuilderWithArguments<TField, TVariables> WithDirective(string name);

  IFieldBuilderWithArguments<TField, TVariables> WithDirective(string name, Expression<Func<IDirectiveBuilder<TVariables>, IDirectiveBuilder<TVariables>>> directive);

  IBuildResult<TResult, TVariables> WithSelection<TResult>(Expression<Func<TField, TResult>> selection);
}
