namespace CodeArchitects.Platform.Actors.Scheduling;

/// <summary>
/// Represents a unique identifier for a scheduled activity.
/// </summary>
public readonly struct ScheduleId : IEquatable<ScheduleId>
{
  private ScheduleId(string id) => Id = id;

  internal string Id { get; }

  /// <inheritdoc/>
  public bool Equals(ScheduleId other) => other.Id == Id;

  /// <inheritdoc/>
  public override bool Equals(object? obj) => obj is ScheduleId other && Equals(other);

  /// <inheritdoc/>
  public override int GetHashCode() => Id is not null
    ? Id.GetHashCode()
    : typeof(ScheduleId).GetHashCode();

  /// <inheritdoc/>
  public override string ToString() => Id;

  // TODO: Needs a consistent random before making it public
  /// <summary>
  /// Creates a new random <see cref="ScheduleId"/>.
  /// </summary>
  /// <returns>A new instance of <see cref="ScheduleId"/>.</returns>
  internal static ScheduleId New() => new(Guid.NewGuid().ToString());

  /// <summary>
  /// Creates a <see cref="ScheduleId"/> from the specified string.
  /// </summary>
  /// <param name="id">The id to use for the schedule.</param>
  /// <returns>A new instance of <see cref="ScheduleId"/>.</returns>
  public static ScheduleId New(string id) => new(id);

  /// <summary>
  /// Implicitly converts a <see cref="ScheduleId"/> to a <see cref="string"/>.
  /// </summary>
  /// <param name="id">The <see cref="ScheduleId"/> to convert.</param>
  /// <returns>The schedule id as a string.</returns>
  public static explicit operator string(ScheduleId id) => id.Id;

  /// <summary>
  /// Determines whether two instances of <see cref="ScheduleId"/> are equal.
  /// </summary>
  /// <param name="left">The first <see cref="ScheduleId"/> to compare.</param>
  /// <param name="right">The second <see cref="ScheduleId"/> to compare.</param>
  /// <returns><see langword="true"/> if the two instances are equal, otherwise <see langword="false"/>.</returns>
  public static bool operator ==(ScheduleId left, ScheduleId right) => left.Equals(right);

  /// <summary>
  /// Determines whether two instances of <see cref="ScheduleId"/> are not equal.
  /// </summary>
  /// <param name="left">The first <see cref="ScheduleId"/> to compare.</param>
  /// <param name="right">The second <see cref="ScheduleId"/> to compare.</param>
  /// <returns><see langword="true"/> if the two instances are not equal, otherwise <see langword="false"/>.</returns>
  public static bool operator !=(ScheduleId left, ScheduleId right) => !(left == right);
}
