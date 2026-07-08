using CodeArchitects.Platform.GraphQL.Document.Builder;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL;

public static partial class GQL
{
  public static TResult Expand<TSource, TResult>(this TSource? source, IBuilder builder, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    where TSource : notnull
    => throw NotImplemented();

  public static IEnumerable<TResult> Expand<TSource, TResult>(this IEnumerable<TSource>? source, IBuilder builder, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    => throw NotImplemented();

  public static IReadOnlyCollection<TResult> Expand<TSource, TResult>(this IReadOnlyCollection<TSource>? source, IBuilder builder, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    => throw NotImplemented();
  
  public static ICollection<TResult> Expand<TSource, TResult>(this ICollection<TSource>? source, IBuilder builder, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    => throw NotImplemented();
  
  public static IReadOnlyList<TResult> Expand<TSource, TResult>(this IReadOnlyList<TSource>? source, IBuilder builder, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    => throw NotImplemented();
  
  public static IList<TResult> Expand<TSource, TResult>(this IList<TSource>? source, IBuilder builder, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    => throw NotImplemented();
  
  public static List<TResult> Expand<TSource, TResult>(this List<TSource>? source, IBuilder builder, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    => throw NotImplemented();
  
  public static TResult[] Expand<TSource, TResult>(this TSource[]? source, IBuilder builder, Expression<Func<IFieldBuilder<TSource>, IBuildResult<TResult>>> expansion)
    => throw NotImplemented();


  public static TResult Expand<TSource, TResult, TVariables>(this TSource? source, IBuilder<TVariables> builder, Expression<Func<IFieldBuilder<TSource, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TSource : notnull
    where TVariables : notnull
    => throw NotImplemented();

  public static IEnumerable<TResult> Expand<TSource, TResult, TVariables>(this IEnumerable<TSource>? source, IBuilder<TVariables> builder, Expression<Func<IFieldBuilder<TSource, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull
    => throw NotImplemented();

  public static IReadOnlyCollection<TResult> Expand<TSource, TResult, TVariables>(this IReadOnlyCollection<TSource>? source, IBuilder<TVariables> builder, Expression<Func<IFieldBuilder<TSource, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull
    => throw NotImplemented();

  public static ICollection<TResult> Expand<TSource, TResult, TVariables>(this ICollection<TSource>? source, IBuilder<TVariables> builder, Expression<Func<IFieldBuilder<TSource, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull
    => throw NotImplemented();

  public static IReadOnlyList<TResult> Expand<TSource, TResult, TVariables>(this IReadOnlyList<TSource>? source, IBuilder<TVariables> builder, Expression<Func<IFieldBuilder<TSource, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull
    => throw NotImplemented();

  public static IList<TResult> Expand<TSource, TResult, TVariables>(this IList<TSource>? source, IBuilder<TVariables> builder, Expression<Func<IFieldBuilder<TSource, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull
  => throw NotImplemented();

  public static List<TResult> Expand<TSource, TResult, TVariables>(this List<TSource>? source, IBuilder<TVariables> builder, Expression<Func<IFieldBuilder<TSource, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull
    => throw NotImplemented();

  public static TResult[] Expand<TSource, TResult, TVariables>(this TSource[]? source, IBuilder<TVariables> builder, Expression<Func<IFieldBuilder<TSource, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull
    => throw NotImplemented();


  public static TResult Select<TSource, TResult>(this TSource? source, IBuilder builder, Expression<Func<TSource, TResult>> selection)
    => throw NotImplemented();

  public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource>? source, IBuilder builder, Expression<Func<TSource, TResult>> selection)
    => throw NotImplemented();

  public static IReadOnlyCollection<TResult> Select<TSource, TResult>(this IReadOnlyCollection<TSource>? source, IBuilder builder, Expression<Func<TSource, TResult>> selection)
    => throw NotImplemented();

  public static ICollection<TResult> Select<TSource, TResult>(this ICollection<TSource>? source, IBuilder builder, Expression<Func<TSource, TResult>> selection)
    => throw NotImplemented();

  public static IReadOnlyList<TResult> Select<TSource, TResult>(this IReadOnlyList<TSource>? source, IBuilder builder, Expression<Func<TSource, TResult>> selection)
    => throw NotImplemented();

  public static IList<TResult> Select<TSource, TResult>(this IList<TSource>? source, IBuilder builder, Expression<Func<TSource, TResult>> selection)
    => throw NotImplemented();

  public static List<TResult> Select<TSource, TResult>(this List<TSource>? source, IBuilder builder, Expression<Func<TSource, TResult>> selection)
    => throw NotImplemented();

  public static TResult[] Select<TSource, TResult>(this TSource[]? source, IBuilder builder, Expression<Func<TSource, TResult>> selection)
    => throw NotImplemented();


  public static TResult Select<TSource, TResult, TVariables>(this TSource? source, IBuilder<TVariables> builder, Expression<Func<TSource, TResult>> selection)
    where TVariables : notnull
    => throw NotImplemented();

  public static IEnumerable<TResult> Select<TSource, TResult, TVariables>(this IEnumerable<TSource>? source, IBuilder<TVariables> builder, Expression<Func<TSource, TResult>> selection)
    where TVariables : notnull
    => throw NotImplemented();

  public static IReadOnlyCollection<TResult> Select<TSource, TResult, TVariables>(this IReadOnlyCollection<TSource>? source, IBuilder<TVariables> builder, Expression<Func<TSource, TResult>> selection)
    where TVariables : notnull
    => throw NotImplemented();

  public static ICollection<TResult> Select<TSource, TResult, TVariables>(this ICollection<TSource>? source, IBuilder<TVariables> builder, Expression<Func<TSource, TResult>> selection)
    where TVariables : notnull
    => throw NotImplemented();

  public static IReadOnlyList<TResult> Select<TSource, TResult, TVariables>(this IReadOnlyList<TSource>? source, IBuilder<TVariables> builder, Expression<Func<TSource, TResult>> selection)
    where TVariables : notnull
    => throw NotImplemented();

  public static IList<TResult> Select<TSource, TResult, TVariables>(this IList<TSource>? source, IBuilder<TVariables> builder, Expression<Func<TSource, TResult>> selection)
    where TVariables : notnull
    => throw NotImplemented();

  public static List<TResult> Select<TSource, TResult, TVariables>(this List<TSource>? source, IBuilder<TVariables> builder, Expression<Func<TSource, TResult>> selection)
    where TVariables : notnull
    => throw NotImplemented();

  public static TResult[] Select<TSource, TResult, TVariables>(this TSource[]? source, IBuilder<TVariables> builder, Expression<Func<TSource, TResult>> selection)
    where TVariables : notnull
    => throw NotImplemented();


  private static NotImplementedException NotImplemented() => new("This method is to be used in document building expressions only.");
}
