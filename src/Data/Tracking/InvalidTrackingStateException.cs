using System.Runtime.Serialization;

namespace CodeArchitects.Platform.Data.Tracking;

[Serializable]
public sealed class InvalidTrackingStateException : Exception
{
  public InvalidTrackingStateException(string entityTypeName, TrackingState state)
    : base($"Invalid tracking state for entity of type '{entityTypeName}': '{state}'.")
  {
    EntityTypeName = entityTypeName;
    State = state;
  }

  private InvalidTrackingStateException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
    EntityTypeName = info.GetString(nameof(EntityTypeName));
    State = (TrackingState)info.GetByte(nameof(State));
  }

  public string EntityTypeName { get; }
  public TrackingState State { get; }

  public override void GetObjectData(SerializationInfo info, StreamingContext context)
  {
    info.AddValue(nameof(EntityTypeName), EntityTypeName);
    info.AddValue(nameof(State), (byte)State);
    base.GetObjectData(info, context);
  }
}
