using System.Runtime.Serialization;

namespace CodeArchitects.Platform.Actors;

/// <summary>
/// Exception thrown when a non-virtual actor is invoked before being initialized.
/// </summary>
[Serializable]
public class UninitializedActorException : Exception
{
  internal UninitializedActorException(Type actorType)
    : base($"Non-virtual actor '{actorType.Name}' was not initialized.")
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="UninitializedActorException"/> class with serialized data.
  /// </summary>
  /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
  /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
  protected UninitializedActorException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}