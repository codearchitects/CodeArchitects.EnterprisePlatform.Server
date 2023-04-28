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

public static class AnnotationExtensions
{
  internal static IConventionAnnotation HasConcurrencyCheck(this IConventionEntityType entityType, TProperty property)
  {
    return entityType.AddAnnotation(ConcurrencyAnnotationNames.ConcurrencyToken, property);
  }

  public static bool TryGetConcurrencyToken(this IEntityType entityType, [NotNullWhen(true)] out TProperty? property)
  {
    return entityType.TryGetAnnotationValue(ConcurrencyAnnotationNames.ConcurrencyToken, out property);
  }
}
