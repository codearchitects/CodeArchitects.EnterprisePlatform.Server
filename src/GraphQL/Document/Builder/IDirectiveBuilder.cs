using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

public interface IDirectiveBuilder
{
  IDirectiveBuilder WithArgument<TArg>(string name, TArg value);
}

public interface IDirectiveBuilder<TVariables>
  where TVariables : notnull
{
  IDirectiveBuilder<TVariables> WithArgument<TArg>(string name, TArg value);

  IDirectiveBuilder<TVariables> WithArgument<TArg>(string name, Expression<Func<TVariables, TArg>> variable);
}
