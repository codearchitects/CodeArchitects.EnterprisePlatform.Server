using System.Reflection;

namespace CodeArchitects.Platform.Actors.Scheduling;

internal interface IActivityManager<TActor>
  where TActor : class
{
  Activity<TActor> CreateActivity(int implementationId, MethodInfo method, IReadOnlyList<object?> arguments);

  Activity<TActor> CreateActivity(int implementationId, string activityName, IReadOnlyList<object?> arguments);
}