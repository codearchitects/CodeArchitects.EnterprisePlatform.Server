using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

public interface IInlineFragmentBuilder<TFragment>
{
  IInlineFragmentBuilder<TFragment> WithDirective(string name);

  IInlineFragmentBuilder<TFragment> WithDirective(string name, Expression<Func<IDirectiveBuilder, IDirectiveBuilder>> directive);

  IBuildResult<TFragment> WithSelection<TSelection>(Expression<Func<TFragment, TSelection>> selection); // TODO: Can't have aliases when using anonymous types as TSelection

  IBuildResult<TFragment> WithSelectionSet<TSelection>(Expression<Func<ISelectionSetBuilder<TFragment>, IBuildResult<TSelection>>> selection);
}

public interface IInlineFragmentBuilder<TFragment, TVariables>
  where TVariables : notnull
{
  IInlineFragmentBuilder<TFragment, TVariables> WithDirective(string name);

  IInlineFragmentBuilder<TFragment, TVariables> WithDirective(string name, Expression<Func<IDirectiveBuilder, IDirectiveBuilder>> directive);

  IBuildResult<TFragment, TVariables> WithSelection<TSelection>(Expression<Func<TFragment, TSelection>> selection);

  IBuildResult<TFragment, TVariables> WithSelectionSet<TSelection>(Expression<Func<ISelectionSetBuilder<TFragment, TVariables>, IBuildResult<TSelection, TVariables>>> selection);
}
