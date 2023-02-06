using System.Reflection;

namespace CodeArchitects.Platform.Actors.Scheduling;

internal interface IActivityManager
{
  ActivityPayload CreatePayload(MethodInfo method, IReadOnlyList<object?> arguments);
  ActivityPayload CreatePayload(string activityName, IReadOnlyList<object?> arguments);
}