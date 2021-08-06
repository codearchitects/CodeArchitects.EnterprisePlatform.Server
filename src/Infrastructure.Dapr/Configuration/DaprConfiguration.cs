namespace CodeArchitects.Platform.Infrastructure.Dapr.Configuration
{
  /// <summary>
  /// Provides configuration options for the Dapr infrastructure.
  /// </summary>
  public class DaprConfiguration
  {
    /// <summary>
    /// Service-specific configuration options.
    /// </summary>
    public ServiceOptions? Service { get; set; }
  }
}
