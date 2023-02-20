namespace CodeArchitects.Platform.Actors.Scheduling;

public static class ActivitySpecStringExtensions
{
  public static ActivitySpec WithArguments(this string activityName, IReadOnlyList<object?> arguments) => new ActivitySpec(activityName, arguments);

  public static ActivitySpec WithArguments(this string activityName, params object?[] arguments) => new ActivitySpec(activityName, arguments);

  public static ActivitySpec WithImplementationType(this string activityName, Type type) => new ActivitySpec(activityName, type);
}
