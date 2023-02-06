namespace CodeArchitects.Platform.Actors.Scheduling;

public readonly struct ScheduleId : IEquatable<ScheduleId>
{
  public ScheduleId(string id)
  {
    Id = id;
  }

  public string Id { get; }

  public bool Equals(ScheduleId other)
  {
    return other.Id == Id;
  }

  public override bool Equals(object obj)
  {
    return obj is ScheduleId other && Equals(other);
  }

  public override int GetHashCode()
  {
    return Id is not null
      ? Id.GetHashCode()
      : typeof(ScheduleId).GetHashCode();
  }

  public override string ToString()
  {
    return Id;
  }

  public static ScheduleId New() => new(Guid.NewGuid().ToString());

  public static implicit operator string(ScheduleId id) => id.Id;

  public static bool operator ==(ScheduleId left, ScheduleId right) => left.Equals(right);

  public static bool operator !=(ScheduleId left, ScheduleId right) => !(left == right);
}
