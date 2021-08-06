namespace CodeArchitects.Platform.Infrastructure.Dapr.State
{
  /// <summary>
  /// Provides state configuration options.
  /// </summary>
  public class DaprStateOptions
  {
    /// <summary>
    /// The default state store name.
    /// </summary>
    public string? DefaultStore { get; init; }
  }
}
