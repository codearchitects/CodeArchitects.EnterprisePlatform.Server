using System.Reflection;

namespace CodeArchitects.Platform.Actors.Scheduling;

internal interface IActivityManager
{
  Activity<TActor> CreateActivity<TActor>(int implementationId, MethodInfo method, IReadOnlyList<object?> arguments)
    where TActor : class;

  Activity<TActor> CreateActivity<TActor>(int implementationId, string activityName, IReadOnlyList<object?> arguments)
    where TActor : class;
}