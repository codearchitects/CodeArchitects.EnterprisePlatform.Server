namespace CodeArchitects.Platform.Common.Identity;

/// <summary>
/// Contains information about the current user.
/// </summary>
public interface IIdentityProfile<TUserId, TTenantId> : IUserProfile<TUserId>, ITenantProfile<TTenantId>
  where TUserId : IEquatable<TUserId>
  where TTenantId : IEquatable<TTenantId>
{
  /// <summary>
  /// <see langword="true"/> if the user is authenticated, <see langword="false"/> otherwise.
  /// </summary>
  bool IsAuthenticated { get; }
}
