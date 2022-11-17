namespace CodeArchitects.Platform.Common.Identity;

/// <summary>
/// Provides the current user's tenant id.
/// </summary>
public interface ITenantProfile<TTenantId>
  where TTenantId : IEquatable<TTenantId>
{
  /// <summary>
  /// The id of the tenant the user belongs to.
  /// </summary>
  TTenantId TenantId { get; }
}
