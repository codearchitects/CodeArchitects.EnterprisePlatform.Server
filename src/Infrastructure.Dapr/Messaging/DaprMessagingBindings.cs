namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging
{
  /// <summary>
  /// Input and output bindings for a message bus.
  /// </summary>
  public class DaprMessagingBindings
  {
    /// <summary>
    /// The name of the bus to subscribe the handler to.
    /// </summary>
    public string? BusName { get; set; }

    /// <summary>
    /// The name of the topic to subscribe the handler to.
    /// </summary>
    public string? Topic { get; set; }
  }
}