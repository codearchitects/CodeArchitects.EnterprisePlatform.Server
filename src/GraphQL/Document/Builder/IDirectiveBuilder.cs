using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

internal interface IDirectiveBuilder
{
  IDirectiveBuilder WithArgument<TArg>(string name, TArg value);
}

internal interface IDirectiveBuilder<TVariables>
  where TVariables : notnull
{
  IDirectiveBuilder<TVariables> WithArgument<TArg>(string name, TArg value);

  IDirectiveBuilder<TVariables> WithArgument<TArg>(string name, Expression<Func<TVariables, TArg>> variable);
}
