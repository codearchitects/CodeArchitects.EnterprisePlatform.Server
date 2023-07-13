using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

public interface ISelector
{
  TResult SelectRef<TSource, TResult>(TSource? source, Expression<Func<TSource, TResult>> selection);

  IEnumerable<TResult> SelectCol<TSource, TResult>(IEnumerable<TSource>? source, Expression<Func<TSource, TResult>> selection);

  IReadOnlyCollection<TResult> SelectCol<TSource, TResult>(IReadOnlyCollection<TSource>? source, Expression<Func<TSource, TResult>> selection);

  ICollection<TResult> SelectCol<TSource, TResult>(ICollection<TSource>? source, Expression<Func<TSource, TResult>> selection);

  IReadOnlyList<TResult> SelectCol<TSource, TResult>(IReadOnlyList<TSource>? source, Expression<Func<TSource, TResult>> selection);

  IList<TResult> SelectCol<TSource, TResult>(IList<TSource>? source, Expression<Func<TSource, TResult>> selection);

  List<TResult> SelectCol<TSource, TResult>(List<TSource>? source, Expression<Func<TSource, TResult>> selection);

  TResult[] SelectCol<TSource, TResult>(TSource[]? source, Expression<Func<TSource, TResult>> selection);
}
