namespace CodeArchitects.Platform.Messaging.Dapr.AspNetCore.Configuration;

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

  public Dictionary<string, List<HandlerBindingsConfig>> Methods { get; set; } = new();
}
