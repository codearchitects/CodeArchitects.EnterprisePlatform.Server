using System.ComponentModel.DataAnnotations;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Components.Schema;

/// <summary>
/// The schema of the metadata section of the component.
/// </summary>
public class MetadataSchema
{
  [Required]
  public string Name { get; set; } = default!;

  public string? Namespace { get; set; }
}
