namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Materialization;

/// <summary>
/// Names for association annotations.
/// Use the extension methods defined in <see cref="MaterializationAnnotationExtensions"/> to retrieve the annotations from the metadata.
/// </summary>
public static class MaterializationAnnotationNames
{
  /// <summary>
  /// The prefix for all annotations.
  /// </summary>
  public const string Prefix = "CA:";

  /// <summary>
  /// The name for default factory annotations.
  /// </summary>
  public const string DefaultFactory = Prefix + nameof(DefaultFactory);
}
