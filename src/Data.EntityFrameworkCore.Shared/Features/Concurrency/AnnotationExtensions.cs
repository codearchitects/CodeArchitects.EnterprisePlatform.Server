using Microsoft.EntityFrameworkCore.Metadata;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Concurrency;

using TProperty =
#if NET6_0_OR_GREATER
  IReadOnlyProperty
#else
  IProperty
#endif
  ;

/// <summary>
/// Extension methods for optimistic concurrency annotations.
/// </summary>
public static class AnnotationExtensions
{
  internal static IConventionAnnotation HasConcurrencyCheck(this IConventionEntityType entityType, TProperty property)
  {
    return entityType.AddAnnotation(ConcurrencyAnnotationNames.ConcurrencyToken, property);
  }

  /// <summary>
  /// Retrieves the property that was configured to serve as a concurrency token, if it exists.
  /// </summary>
  /// <param name="entityType">The entity.</param>
  /// <param name="property">The concurrency token property, if it exists, or <see langword="null"/>.</param>
  /// <returns><see langword="true"/> if the concurrency token property exists, <see langword="false"/> otherwise.</returns>
  public static bool TryGetConcurrencyToken(this IEntityType entityType, [NotNullWhen(true)] out TProperty? property)
  {
    return entityType.TryGetAnnotationValue(ConcurrencyAnnotationNames.ConcurrencyToken, out property);
  }
}
