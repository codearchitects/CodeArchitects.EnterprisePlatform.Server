namespace CodeArchitects.Platform.Messaging.Dapr.AspNetCore.Configuration;

/// <summary>
/// Input and output bindings for a message handler.
/// </summary>
public class HandlerBindingsConfig
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
  /// The list of the output actions the handler will be bound to.
  /// </summary>
  public List<OutputBindingConfig> Output { get; set; } = new();
}
