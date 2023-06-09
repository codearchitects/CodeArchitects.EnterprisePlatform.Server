using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

internal interface IExpander
{
  TResult ExpandRef<TSource, TResult>(TSource? source, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    where TSource : notnull;

  IEnumerable<TResult> ExpandCol<TSource, TResult>(IEnumerable<TSource>? source, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    where TSource : notnull;

  IReadOnlyCollection<TResult> ExpandCol<TSource, TResult>(IReadOnlyCollection<TSource>? source, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    where TSource : notnull;

  ICollection<TResult> ExpandCol<TSource, TResult>(ICollection<TSource>? source, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    where TSource : notnull;

  IReadOnlyList<TResult> ExpandCol<TSource, TResult>(IReadOnlyList<TSource>? source, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    where TSource : notnull;

  IList<TResult> ExpandCol<TSource, TResult>(IList<TSource>? source, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    where TSource : notnull;

  List<TResult> ExpandCol<TSource, TResult>(List<TSource>? source, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    where TSource : notnull;

  TResult[] ExpandCol<TSource, TResult>(TSource[]? source, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    where TSource : notnull;
}

internal interface IExpander<TVariables>
  where TVariables : notnull
{
  TResult ExpandRef<TSource, TResult>(TSource? source, Expression<Func<IFieldBuilder<TSource, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TSource : notnull;

  IEnumerable<TResult> ExpandCol<TSource, TResult>(IEnumerable<TSource>? source, Expression<Func<IFieldBuilder<TSource, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TSource : notnull;

  IReadOnlyCollection<TResult> ExpandCol<TSource, TResult>(IReadOnlyCollection<TSource>? source, Expression<Func<IFieldBuilder<TSource, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TSource : notnull;

  ICollection<TResult> ExpandCol<TSource, TResult>(ICollection<TSource>? source, Expression<Func<IFieldBuilder<TSource, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TSource : notnull;

  IReadOnlyList<TResult> ExpandCol<TSource, TResult>(IReadOnlyList<TSource>? source, Expression<Func<IFieldBuilder<TSource, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TSource : notnull;

  IList<TResult> ExpandCol<TSource, TResult>(IList<TSource>? source, Expression<Func<IFieldBuilder<TSource, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TSource : notnull;

  List<TResult> ExpandCol<TSource, TResult>(List<TSource>? source, Expression<Func<IFieldBuilder<TSource, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TSource : notnull;

  TResult[] ExpandCol<TSource, TResult>(TSource[]? source, Expression<Func<IFieldBuilder<TSource, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TSource : notnull;
}
