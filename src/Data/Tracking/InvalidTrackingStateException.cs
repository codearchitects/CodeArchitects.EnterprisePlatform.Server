using System.Runtime.Serialization;

namespace CodeArchitects.Platform.Data.Tracking;

/// <summary>
/// The exception that is thrown when an invalid tracking state is encountered.
/// </summary>
[Serializable]
public sealed class InvalidTrackingStateException : Exception
{
  /// <summary>
  /// Initializes a new instance of the <see cref="InvalidTrackingStateException"/> class with the specified entity type name and tracking state.
  /// </summary>
  /// <param name="entityTypeName">The name of the entity type that the tracking state is invalid for.</param>
  /// <param name="state">The invalid tracking state.</param>
  public InvalidTrackingStateException(string entityTypeName, TrackingState state)
    : base($"Invalid tracking state for entity of type '{entityTypeName}': '{state}'.")
  {
    EntityTypeName = entityTypeName;
    State = state;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="InvalidTrackingStateException"/> class with serialized data.
  /// </summary>
  /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.</param>
  /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
  private InvalidTrackingStateException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
    EntityTypeName = info.GetString(nameof(EntityTypeName));
    State = (TrackingState)info.GetByte(nameof(State));
  }

  /// <summary>
  /// Gets the name of the entity type that the tracking state is invalid for.
  /// </summary>
  public string EntityTypeName { get; }

  /// <summary>
  /// Gets the invalid tracking state.
  /// </summary>
  public TrackingState State { get; }

  /// <inheritdoc/>
  public override void GetObjectData(SerializationInfo info, StreamingContext context)
  {
    info.AddValue(nameof(EntityTypeName), EntityTypeName);
    info.AddValue(nameof(State), (byte)State);
    base.GetObjectData(info, context);
  }
}
