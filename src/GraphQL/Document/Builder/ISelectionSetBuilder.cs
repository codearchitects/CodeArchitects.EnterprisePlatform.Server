using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

public interface ISelectionSetBuilder<TField> : IBuilder
{
  ISelectionSetBuilderWithSelection<TResult> WithSelection<TResult>(Expression<Func<TField, TResult>> selection);
}

public interface ISelectionSetBuilderWithSelection<TField> : IBuildResult<TField>, IBuilder
{
  ISelectionSetBuilderWithSelection<TField> WithFragmentSpread(GraphFragment<TField> fragment);

  ISelectionSetBuilderWithSelection<TField> WithFragmentSpread(GraphFragment<TField> fragment, Expression<Func<IFragmentSpreadBuilder<TField>, IFragmentSpreadBuilder<TField>>> buildFragment);

  ISelectionSetBuilderWithSelection<TField> WithInlineFragment(Expression<Func<IInlineFragmentBuilder<TField>, IBuildResult<TField>>> buildFragment);

  ISelectionSetBuilderWithSelection<TField> WithInlineFragment<TFragment>(Expression<Func<IInlineFragmentBuilder<TFragment>, IBuildResult<TFragment>>> buildFragment)
    where TFragment : TField;
}

public interface ISelectionSetBuilder<TField, TVariables> : IBuilder<TVariables>
  where TVariables : notnull
{
  ISelectionSetBuilderWithSelection<TResult, TVariables> WithSelection<TResult>(Expression<Func<TField, TResult>> selection);
}

public interface ISelectionSetBuilderWithSelection<TField, TVariables> : IBuildResult<TField, TVariables>, IBuilder<TVariables>
  where TVariables : notnull
{
  ISelectionSetBuilderWithSelection<TField, TVariables> WithFragmentSpread(GraphFragment<TField> fragment);

  ISelectionSetBuilderWithSelection<TField, TVariables> WithFragmentSpread(GraphFragment<TField> fragment, Expression<Func<IFragmentSpreadBuilder<TField, TVariables>, IFragmentSpreadBuilder<TField, TVariables>>> buildFragment);

  ISelectionSetBuilderWithSelection<TField, TVariables> WithFragmentSpread(GraphFragment<TField, TVariables> fragment);

  ISelectionSetBuilderWithSelection<TField, TVariables> WithFragmentSpread(GraphFragment<TField, TVariables> fragment, Expression<Func<IFragmentSpreadBuilder<TField, TVariables>, IFragmentSpreadBuilder<TField, TVariables>>> buildFragment);

  ISelectionSetBuilderWithSelection<TField, TVariables> WithInlineFragment(Expression<Func<IInlineFragmentBuilder<TField, TVariables>, IBuildResult<TField, TVariables>>> buildFragment);

  ISelectionSetBuilderWithSelection<TField, TVariables> WithInlineFragment<TFragment>(Expression<Func<IInlineFragmentBuilder<TFragment, TVariables>, IBuildResult<TFragment, TVariables>>> buildFragment)
    where TFragment : TField;
}
