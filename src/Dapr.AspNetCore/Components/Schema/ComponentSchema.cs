using System.ComponentModel.DataAnnotations;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Components.Schema;

/// <summary>
/// The schema of a Dapr component file.
/// </summary>
public class ComponentSchema
{
  [Required]
  public string ApiVersion { get; set; } = default!;

  [Required]
  public string Kind { get; set; } = default!;

  [Required]
  public MetadataSchema Metadata { get; set; } = default!;

  [Required]
  public SpecSchema Spec { get; set; } = default!;
}
