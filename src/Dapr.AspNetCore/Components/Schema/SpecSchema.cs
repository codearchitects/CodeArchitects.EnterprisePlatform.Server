using System.ComponentModel.DataAnnotations;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Components.Schema;

public class SpecSchema
{
  [Required]
  public string Type { get; init; } = default!;

  public string? Version { get; init; }

  public MetadataItemSchema[]? Metadata { get; init; }
}
