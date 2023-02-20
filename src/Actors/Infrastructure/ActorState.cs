using System.Text.Json.Serialization;

namespace CodeArchitects.Platform.Actors.Infrastructure;

internal abstract class ActorState
{
  public abstract int ImplementationId { get; set; }

  [JsonPropertyName("bdgs")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
  public int EnabledBindings { get; set; }

  public bool IsBindingEnabled(int index)
  {
    return (EnabledBindings & (1 << index)) != 0;
  }
}
