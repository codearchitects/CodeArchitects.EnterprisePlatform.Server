using CodeArchitects.Platform.Infrastructure.Dapr.Messaging;
using CodeArchitects.Platform.Infrastructure.Dapr.State;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Configuration;

/// <summary>
/// Provides service-specific configuration options for the Dapr infrastructure.
/// </summary>
public class ServiceOptions
{
  /// <summary>
  /// Path to the Dapr component folder
  /// </summary>
  public string? ComponentFolderPath { get; set; }

  /// <summary>
  /// The messaging options.
  /// </summary>
  public DaprMessagingOptions? Messaging { get; set; }

  /// <summary>
  /// The state options.
  /// </summary>
  public DaprStateOptions? State { get; set; }
}
