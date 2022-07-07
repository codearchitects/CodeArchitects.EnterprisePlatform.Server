namespace CodeArchitects.Platform.Messaging.AspNetCore.Configuration;

/// <summary>
/// Provides messaging configuration options.
/// </summary>
public class MessagingConfig
{
  /// <summary>
  /// The default bus name.
  /// </summary>
  public string? DefaultBus { get; set; }

  /// <summary>
  /// Declarative bindings for the handlers.
  /// </summary>
  public Dictionary<string, HandlerClassBindingsConfig> Bindings { get; set; } = new();
}
