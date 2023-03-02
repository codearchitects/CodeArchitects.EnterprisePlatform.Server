namespace CodeArchitects.Platform.Actors.Scheduling;

/// <summary>
/// Represents options for scheduling an actor activity.
/// </summary>
/// <param name="Timer">The interval to wait before the activity is executed for the first time.</param>
/// <param name="Period">The interval between executions of the activity.</param>
public record SchedulingOptions(TimeSpan Timer, TimeSpan Period)
{
  /// <summary>
  /// Options for an activity that will execute immediately and never repeat.
  /// </summary>
  public static readonly SchedulingOptions Now = new(TimeSpan.Zero, Timeout.InfiniteTimeSpan);

  /// <summary>
  /// Creates a new <see cref="SchedulingOptions"/> instance with the specified timer interval.
  /// </summary>
  /// <param name="timer">The interval to wait before the activity is executed for the first time.</param>
  /// <returns>A new instance of <see cref="SchedulingOptions"/>.</returns>
  public static SchedulingOptions In(TimeSpan timer) => new(timer, Timeout.InfiniteTimeSpan);

  /// <summary>
  /// Creates a new <see cref="SchedulingOptions"/> instance with the specified period.
  /// </summary>
  /// <param name="period">The interval between executions of the activity.</param>
  /// <returns>A new instance of <see cref="SchedulingOptions"/>.</returns>
  public static SchedulingOptions Every(TimeSpan period) => new(TimeSpan.Zero, period);
}
