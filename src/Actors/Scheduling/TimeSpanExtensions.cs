namespace CodeArchitects.Platform.Actors.Scheduling;

public static class TimeSpanExtensions
{
  public static TimeSpan Hours(this int hours) => TimeSpan.FromHours(hours);

  public static TimeSpan Hours(this double hours) => TimeSpan.FromHours(hours);

  public static TimeSpan Minutes(this int minutes) => TimeSpan.FromMinutes(minutes);

  public static TimeSpan Minutes(this double minutes) => TimeSpan.FromMinutes(minutes);

  public static TimeSpan Seconds(this int seconds) => TimeSpan.FromSeconds(seconds);

  public static TimeSpan Seconds(this double seconds) => TimeSpan.FromSeconds(seconds);
}
