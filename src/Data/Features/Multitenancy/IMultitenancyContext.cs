namespace CodeArchitects.Platform.Data.Features.Multitenancy;

/// <summary>
/// Provides information about multitenancy.
/// </summary>
public interface IMultitenancyContext
{
  /// <summary>
  /// Indicates whether the data should be filtered based on the current tenant.
  /// </summary>
  bool ShouldFilter { get; }

  /// <summary>
  /// The identifier of the current tenant.
  /// </summary>
  object TenantId { get; }
}
