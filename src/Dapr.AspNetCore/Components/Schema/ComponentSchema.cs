using System.ComponentModel.DataAnnotations;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Components.Schema;

public class ComponentSchema
{
  [Required]
  public string ApiVersion { get; init; } = default!;

  [Required]
  public string Kind { get; init; } = default!;

  [Required]
  public MetadataSchema Metadata { get; init; } = default!;

  [Required]
  public SpecSchema Spec { get; init; } = default!;
}
