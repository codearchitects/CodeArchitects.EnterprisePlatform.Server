namespace CodeArchitects.Platform.Infrastructure.Dapr.Configuration.Component;

internal class ComponentSchema
{
  public string? ApiVersion { get; set; }
  public string? Kind { get; set; }
  public MetadataSchema? Metadata { get; set; }
  public SpecSchema? Spec { get; set; }
}
