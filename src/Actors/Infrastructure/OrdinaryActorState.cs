using System.Text.Json.Serialization;

namespace CodeArchitects.Platform.Actors.Infrastructure;

internal class OrdinaryActorState : ActorState
{
  [JsonIgnore]
  public override int ImplementationId
  {
    get => 0;
    set => throw new InvalidOperationException("Actor is not polymorphic.");
  }
}
