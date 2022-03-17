namespace CodeArchitects.Platform.Infrastructure.Dapr.Configuration.Components;

internal class SpecSchema
{
  public string? Type { get; set; }
  public string? Version { get; set; }
  public MetadataItemSchema[]? Metadata { get; set; }
}
