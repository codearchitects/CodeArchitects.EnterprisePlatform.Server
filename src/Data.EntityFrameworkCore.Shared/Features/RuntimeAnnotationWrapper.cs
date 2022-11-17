namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features;

public record RuntimeAnnotationWrapper<TAnnotation>(TAnnotation Annotation)
  where TAnnotation : notnull;

public static class RuntimeAnnotationWrapper
{
  public static RuntimeAnnotationWrapper<TAnnotation> Create<TAnnotation>(TAnnotation annotation)
    where TAnnotation : notnull
  {
    if (annotation is null)
      throw new ArgumentException(nameof(annotation));

    return new(annotation);
  }
}