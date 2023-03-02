namespace CodeArchitects.Platform.Actors.Scheduling;

/// <summary>
/// Extension methods for specifying time durations.
/// </summary>
public static class TimeSpanExtensions
{
  /// <summary>
  /// Returns a <see cref="TimeSpan"/> that represents a specified number of hours.
  /// </summary>
  /// <param name="hours">The number of hours.</param>
  /// <returns>A <see cref="TimeSpan"/> that represents a specified number of hours.</returns>
  public static TimeSpan Hours(this int hours) => TimeSpan.FromHours(hours);

  /// <summary>
  /// Returns a <see cref="TimeSpan"/> that represents a specified number of hours.
  /// </summary>
  /// <param name="hours">The number of hours.</param>
  /// <returns>A <see cref="TimeSpan"/> that represents a specified number of hours.</returns>
  public static TimeSpan Hours(this double hours) => TimeSpan.FromHours(hours);

  /// <summary>
  /// Returns a <see cref="TimeSpan"/> that represents a specified number of minutes.
  /// </summary>
  /// <param name="minutes">The number of minutes.</param>
  /// <returns>A <see cref="TimeSpan"/> that represents a specified number of minutes.</returns>
  public static TimeSpan Minutes(this int minutes) => TimeSpan.FromMinutes(minutes);

  /// <summary>
  /// Returns a <see cref="TimeSpan"/> that represents a specified number of minutes.
  /// </summary>
  /// <param name="minutes">The number of minutes.</param>
  /// <returns>A <see cref="TimeSpan"/> that represents a specified number of minutes.</returns>
  public static TimeSpan Minutes(this double minutes) => TimeSpan.FromMinutes(minutes);

  /// <summary>
  /// Returns a <see cref="TimeSpan"/> that represents a specified number of seconds.
  /// </summary>
  /// <param name="seconds">The number of seconds.</param>
  /// <returns>A <see cref="TimeSpan"/> that represents a specified number of seconds.</returns>
  public static TimeSpan Seconds(this int seconds) => TimeSpan.FromSeconds(seconds);

  /// <summary>
  /// Returns a <see cref="TimeSpan"/> that represents a specified number of seconds.
  /// </summary>
  /// <param name="seconds">The number of seconds.</param>
  /// <returns>A <see cref="TimeSpan"/> that represents a specified number of seconds.</returns>
  public static TimeSpan Seconds(this double seconds) => TimeSpan.FromSeconds(seconds);
}
