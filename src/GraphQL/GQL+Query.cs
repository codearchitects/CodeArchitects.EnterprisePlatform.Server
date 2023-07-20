using CodeArchitects.Platform.GraphQL.Document.Builder;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL;

public static partial class GQL
{
  public static TResult ExpandRef<TSource, TResult>(TSource? source, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    where TSource : notnull
    => throw NotImplemented();

  public static IEnumerable<TResult> ExpandCol<TSource, TResult>(IEnumerable<TSource>? source, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    => throw NotImplemented();

  public static IReadOnlyCollection<TResult> ExpandCol<TSource, TResult>(IReadOnlyCollection<TSource>? source, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    => throw NotImplemented();

  public static ICollection<TResult> ExpandCol<TSource, TResult>(ICollection<TSource>? source, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    => throw NotImplemented();

  public static IReadOnlyList<TResult> ExpandCol<TSource, TResult>(IReadOnlyList<TSource>? source, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    => throw NotImplemented();

  public static IList<TResult> ExpandCol<TSource, TResult>(IList<TSource>? source, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    => throw NotImplemented();

  public static List<TResult> ExpandCol<TSource, TResult>(List<TSource>? source, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    => throw NotImplemented();

  public static TResult[] ExpandCol<TSource, TResult>(TSource[]? source, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    => throw NotImplemented();


  public static TResult SelectRef<TSource, TResult>(TSource? source, Expression<Func<TSource, TResult>> selection)
    => throw NotImplemented();

  public static IEnumerable<TResult> SelectCol<TSource, TResult>(IEnumerable<TSource>? source, Expression<Func<TSource, TResult>> selection)
    => throw NotImplemented();

  public static IReadOnlyCollection<TResult> SelectCol<TSource, TResult>(IReadOnlyCollection<TSource>? source, Expression<Func<TSource, TResult>> selection)
    => throw NotImplemented();

  public static ICollection<TResult> SelectCol<TSource, TResult>(ICollection<TSource>? source, Expression<Func<TSource, TResult>> selection)
    => throw NotImplemented();

  public static IReadOnlyList<TResult> SelectCol<TSource, TResult>(IReadOnlyList<TSource>? source, Expression<Func<TSource, TResult>> selection)
    => throw NotImplemented();

  public static IList<TResult> SelectCol<TSource, TResult>(IList<TSource>? source, Expression<Func<TSource, TResult>> selection)
    => throw NotImplemented();

  public static List<TResult> SelectCol<TSource, TResult>(List<TSource>? source, Expression<Func<TSource, TResult>> selection)
    => throw NotImplemented();

  public static TResult[] SelectCol<TSource, TResult>(TSource[]? source, Expression<Func<TSource, TResult>> selection)
    => throw NotImplemented();


  private static NotImplementedException NotImplemented() => new("This method is to be used in document building expressions only.");
}
