using System.Text.Json.Serialization;

namespace CodeArchitects.Platform.Actors.Scheduling;

internal abstract class Activity
{
  public abstract int Id { get; }

  [JsonPropertyName(":impl")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
  public int ImplementationId { get; set; }
}
