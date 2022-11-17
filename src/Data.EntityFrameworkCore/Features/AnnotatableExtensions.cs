using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features;

internal static class AnnotatableExtensions
{
  public static bool TryGetRuntimeAnnotationValue<TValue>(this IAnnotatable annotatable, string annotationName, [NotNullWhen(true)] out TValue? value)
  {
    IAnnotation? annotation = annotatable.FindRuntimeAnnotation(annotationName);
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
