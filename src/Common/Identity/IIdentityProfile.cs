namespace CodeArchitects.Platform.Common.Identity;

/// <summary>
/// Contains information about the current user.
/// </summary>
public interface IIdentityProfile
{
  /// <summary>
  /// <c>true</c> if the user is authenticated, <c>false</c> otherwise.
  /// </summary>
  bool IsAuthenticated { get; }

  /// <summary>
  /// The id of the user.
  /// </summary>
  Guid UserId { get; }

  /// <summary>
  /// The id of the tenant the user belongs to.
  /// </summary>
  Guid TenantId { get; }
}
