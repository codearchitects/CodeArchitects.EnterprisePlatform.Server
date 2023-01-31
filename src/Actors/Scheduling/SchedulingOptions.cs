namespace CodeArchitects.Platform.Actors.Scheduling;

public record SchedulingOptions(
  ScheduleId Id,
  TimeSpan Timer,
  TimeSpan Period,
  bool Pesistent)
{
  public static readonly SchedulingOptions Now = new();

  public SchedulingOptions()
    : this(default, TimeSpan.Zero, Timeout.InfiniteTimeSpan, true)
  {
  }

  public static SchedulingOptions In(TimeSpan timer) => new() { Timer = timer };

  public static SchedulingOptions Every(TimeSpan period) => new() { Period = period };
}
