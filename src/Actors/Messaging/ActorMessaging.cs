using System.Reflection;

namespace CodeArchitects.Platform.Actors.Messaging;

/// <summary>
/// Defines utilities for using messaging with actors.
/// </summary>
public static class ActorMessaging
{
  /// <summary>
  /// The assembly that contains the actual message handlers that will map to application's endpoints.
  /// </summary>
  public static Assembly Assembly => MessagingAssembly;

  internal static MessagingAssembly MessagingAssembly { get; } = new();
}
