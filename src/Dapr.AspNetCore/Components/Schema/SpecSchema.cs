using System.ComponentModel.DataAnnotations;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Components.Schema;

/// <summary>
/// The schema of the spec section of the component.
/// </summary>
public class SpecSchema
{
  [Required]
  public string Type { get; set; } = default!;

  public string? Version { get; set; }

  public MetadataItemSchema[]? Metadata { get; set; }
}
