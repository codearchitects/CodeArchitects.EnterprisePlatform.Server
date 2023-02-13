using System.Text.Json.Serialization;

namespace CodeArchitects.Platform.Actors.Infrastructure;

internal class PolymorphicActorState : ActorState
{
  [JsonPropertyName("impl")]
  public override int ImplementationId { get; set; }
}
