namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.SoftDelete;

/// <summary>
/// Names for soft delete annotations.
/// </summary>
public static class SoftDeleteAnnotationNames
{
  /// <summary>
  /// The prefix for all annotations.
  /// </summary>
  public const string Prefix = "CA:SoftDelete:";

  /// <summary>
  /// The name of the predicate annotation.
  /// </summary>
  public const string Predicate = Prefix + nameof(Predicate);

  /// <summary>
  /// The name of the property name annotation.
  /// </summary>
  public const string PropertyName = Prefix + nameof(PropertyName);

  /// <summary>
  /// The name of the property annotation.
  /// </summary>
  public const string Property = Prefix + nameof(Property);

  /// <summary>
  /// The name of the column mappings annotation.
  /// </summary>
  public const string ColumnMappings = Prefix + nameof(ColumnMappings);
}
