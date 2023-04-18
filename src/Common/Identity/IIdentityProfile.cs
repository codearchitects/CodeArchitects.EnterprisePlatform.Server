namespace CodeArchitects.Platform.Common.Identity;

/// <summary>
/// Contains information about the current user.
/// </summary>
public interface IIdentityProfile<TUserId>
  where TUserId : IEquatable<TUserId>
{
  /// <summary>
  /// <see langword="true"/> if the user is authenticated, <see langword="false"/> otherwise.
  /// </summary>
  bool IsAuthenticated { get; }

  /// <summary>
  /// The id of the user.
  /// </summary>
  TUserId UserId { get; }
}
