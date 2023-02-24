using System.Reflection;

namespace CodeArchitects.Platform.Actors.Messaging;

public static class ActorMessaging
{
  public static Assembly Assembly => MessagingAssembly;

  internal static MessagingAssembly MessagingAssembly { get; } = new();
}
