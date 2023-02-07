using System.Runtime.Serialization;

namespace CodeArchitects.Platform.Actors;

[Serializable]
public class UninitializedActorException : Exception
{
  internal UninitializedActorException(Type actorType)
    : base($"Non-virtual actor '{actorType.Name}' was not initialized.")
  {
  }

  protected UninitializedActorException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}