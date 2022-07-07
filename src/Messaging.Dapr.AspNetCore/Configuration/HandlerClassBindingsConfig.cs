namespace CodeArchitects.Platform.Messaging.Dapr.AspNetCore.Configuration;

/// <summary>
/// Input and output bindings for a message handler.
/// </summary>
public class HandlerClassBindingsConfig
{
  /// <summary>
  /// The name of the bus to subscribe the handler to.
  /// </summary>
  public string? Bus { get; set; }

  /// <summary>
  /// The name of the topic to subscribe the handler to.
  /// </summary>
  public string? Topic { get; set; }

  /// <summary>
  /// Declarative bindings for the handler methods.
  /// </summary>
  public Dictionary<string, List<HandlerBindingsConfig>> Methods { get; set; } = new();
}
