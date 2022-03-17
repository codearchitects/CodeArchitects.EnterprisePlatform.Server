namespace CodeArchitects.Platform.Infrastructure.Dapr.Configuration.Component;

internal class SpecSchema
{
  public string? Type { get; set; }
  public string? Version { get; set; }
  public MetadataItemSchema[]? Metadata { get; set; }
}
