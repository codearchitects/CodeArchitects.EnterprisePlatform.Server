namespace CodeArchitects.Platform.Infrastructure.Dapr.State;

/// <summary>
/// Provides state configuration options.
/// </summary>
public class DaprStateConfig
{
  /// <summary>
  /// The default state store name.
  /// </summary>
  public string? DefaultStore { get; set; }
}
