using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

public interface IFragmentBuilder<TField>
{
  IFragmentBuilder<TField> WithDirective(string name);

  IFragmentBuilder<TField> WithDirective(string name, Expression<Func<IDirectiveBuilder, IDirectiveBuilder>> directive);

  IBuildResult<TField> WithSelection(Expression<Func<TField, TField>> selection);

  IBuildResult<TField> WithSelectionSet(Expression<Func<ISelectionSetBuilder<TField>, IBuildResult<TField>>> selection);
}

public interface IFragmentBuilder<TField, TVariables>
  where TVariables : notnull
{
  IFragmentBuilder<TField> WithDirective(string name);

  IFragmentBuilder<TField> WithDirective(string name, Expression<Func<IDirectiveBuilder<TVariables>, IDirectiveBuilder<TVariables>>> directive);

  IBuildResult<TField, TVariables> WithSelection(Expression<Func<TField, TField>> selection);

  IBuildResult<TField, TVariables> WithSelectionSet(Expression<Func<ISelectionSetBuilder<TField, TVariables>, IBuildResult<TField, TVariables>>> selection);
}
