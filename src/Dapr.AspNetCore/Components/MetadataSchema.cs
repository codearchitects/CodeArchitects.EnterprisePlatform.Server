using System.ComponentModel.DataAnnotations;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Components;

public class MetadataSchema
{
  [Required]
  public string Name { get; init; } = default!;

  public string? Namespace { get; init; }
}
