namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging;

/// <summary>
/// Provides messaging configuration options.
/// </summary>
public class DaprMessagingOptions
{
  /// <summary>
  /// The default bus name.
  /// </summary>
  public string? DefaultBus { get; init; }
}
