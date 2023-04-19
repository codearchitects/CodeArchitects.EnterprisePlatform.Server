namespace CodeArchitects.Platform.Actors.Scheduling;

/// <summary>
/// Extension methods for creating instances of <see cref="ActivitySpec"/> from strings and optional arguments.
/// </summary>
public static class ActivitySpecStringExtensions
{
  /// <summary>
  /// Specifies the arguments of the activity method.
  /// </summary>
  /// <param name="activityName">The name of the method to execute.</param>
  /// <param name="arguments">The arguments to pass to the activity.</param>
  /// <returns>A new instance of the <see cref="ActivitySpec"/> class with the specified arguments.</returns>
  public static ActivitySpec WithArguments(this string activityName, IReadOnlyList<object?> arguments) => new ActivitySpec(activityName, arguments);

  /// <summary>
  /// Specifies the arguments of the activity method.
  /// </summary>
  /// <param name="activityName">The name of the method to execute.</param>
  /// <param name="arguments">The arguments to pass to the activity.</param>
  /// <returns>A new instance of the <see cref="ActivitySpec"/> class with the specified arguments.</returns>
  public static ActivitySpec WithArguments(this string activityName, params object?[] arguments) => new ActivitySpec(activityName, arguments);

  /// <summary>
  /// Specifies the implementation type to use for the activity.
  /// </summary>
  /// <param name="activityName"></param>
  /// <param name="implementationType">The type of actor implementation to use for the activity.</param>
  /// <returns>A new instance of the <see cref="ActivitySpec"/> class with the implementation type.</returns>
  public static ActivitySpec WithImplementationType(this string activityName, Type implementationType) => new ActivitySpec(activityName, implementationType);
}
