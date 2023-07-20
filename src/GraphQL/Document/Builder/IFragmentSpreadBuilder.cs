using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

public interface IFragmentSpreadBuilder<TField>
{
  IFragmentSpreadBuilder<TField> WithDirective(string name);

  IFragmentSpreadBuilder<TField> WithDirective(string name, Expression<Func<IDirectiveBuilder, IDirectiveBuilder>> directive);
}

public interface IFragmentSpreadBuilder<TField, TVariables>
  where TVariables : notnull
{
  IFragmentSpreadBuilder<TField, TVariables> WithDirective(string name);

  IFragmentSpreadBuilder<TField, TVariables> WithDirective(string name, Expression<Func<IDirectiveBuilder<TVariables>, IDirectiveBuilder<TVariables>>> directive);
}
