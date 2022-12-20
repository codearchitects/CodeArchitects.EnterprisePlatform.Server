namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Associations;

/// <summary>
/// Names for association annotations.
/// Use the <see cref="AssociationsAnnotationExtensions.IsComposition"/> and <see cref="AssociationsAnnotationExtensions.IsAggregation"/> methods to retrieve the annotations from the metadata.
/// </summary>
public static class AssociationsAnnotationNames
{
  /// <summary>
  /// The prefix for all annotations.
  /// </summary>
  public const string Prefix = "CA:";

  /// <summary>
  /// The name for association kind annotations.
  /// </summary>
  public const string AssociationKind = Prefix + nameof(AssociationKind);
}
