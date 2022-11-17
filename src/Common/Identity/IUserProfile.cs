namespace CodeArchitects.Platform.Common.Identity;

/// <summary>
/// Provides the current user's id.
/// </summary>
public interface IUserProfile<TUserId>
  where TUserId : IEquatable<TUserId>
{
  /// <summary>
  /// The id of the user.
  /// </summary>
  TUserId UserId { get; }
}
