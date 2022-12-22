using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features;

using TAnnotatable =
#if NET6_0_OR_GREATER
  IReadOnlyAnnotatable
#else
  IAnnotatable
#endif
  ;

internal static class AnnotatableExtensions
{
  public static bool TryGetAnnotationValue<TValue>(this TAnnotatable annotatable, string annotationName, [NotNullWhen(true)] out TValue? value)
    where TValue : notnull
  {
    IAnnotation? annotation = annotatable.FindAnnotation(annotationName);
    if (annotation is null)
    {
      value = default;
      return false;
    }

    if (annotation.Value is not TValue annotationValue)
      throw new InvalidOperationException($"Annotation '{annotationName}' has the wrong type.");

    value = annotationValue;
    return true;
  }
}
