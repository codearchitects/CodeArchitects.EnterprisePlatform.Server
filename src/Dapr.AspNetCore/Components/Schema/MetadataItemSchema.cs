namespace CodeArchitects.Platform.Dapr.AspNetCore.Components.Schema;

/// <summary>
/// The schema of a Dapr metadata item.
/// </summary>
public class MetadataItemSchema
{
  public string? Name { get; init; }
  public object? Value { get; init; }
}
