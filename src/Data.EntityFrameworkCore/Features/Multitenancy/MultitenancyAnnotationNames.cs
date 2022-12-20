namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

/// <summary>
/// Names for multitenancy annotations.
/// </summary>
public static class MultitenancyAnnotationNames
{
  /// <summary>
  /// The prefix for all annotations.
  /// </summary>
  public const string Prefix = "CA:Multitenancy:";

  /// <summary>
  /// The name of the tenant id annotation.
  /// </summary>
  public const string TenantIdType = Prefix + nameof(TenantIdType);
  
  /// <summary>
  /// The name of the predicate template annotation.
  /// </summary>
  public const string PredicateTemplate = Prefix + nameof(PredicateTemplate);

  /// <summary>
  /// The name of the property name annotation.
  /// </summary>
  public const string PropertyName = Prefix + nameof(PropertyName);

  /// <summary>
  /// The name of the property annotation.
  /// </summary>
  public const string Property = Prefix + nameof(Property);

  /// <summary>
  /// The name of the column mapping annotation.
  /// </summary>
  public const string ColumnMappings = Prefix + nameof(ColumnMappings);
}
