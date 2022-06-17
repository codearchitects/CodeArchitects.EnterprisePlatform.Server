using System;

namespace CodeArchitects.Platform.Data;

/// <summary>
/// Represents an entity that belongs to a tenant.
/// </summary>
public interface ITenantEntity
{
  /// <summary>
  /// The id of the tenant of this entity.
  /// </summary>
  Guid? TenantId { get; set; }
}
