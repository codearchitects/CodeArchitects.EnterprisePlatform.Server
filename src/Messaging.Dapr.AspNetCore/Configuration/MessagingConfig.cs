namespace CodeArchitects.Platform.Messaging.Dapr.AspNetCore.Configuration;

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

  /// <summary>
  /// The names of all the message busses.
  /// </summary>
  public HashSet<string>? BusNames { get; set; }

  internal string? GetDefaultBus()
  {
    return DefaultBus is not null
      ? DefaultBus
      : BusNames is { Count: 1 }
        ? BusNames.Single()
        : null;
  }
}
