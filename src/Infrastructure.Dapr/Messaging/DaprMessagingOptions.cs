using System.Collections.Generic;
using System.Linq;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging;

/// <summary>
/// Provides messaging configuration options.
/// </summary>
public class DaprMessagingOptions
{
  /// <summary>
  /// The default bus name.
  /// </summary>
  public string? DefaultBus { get; set; }

  /// <summary>
  /// Declarative bindings for the handlers.
  /// </summary>
  public Dictionary<string, DaprMessagingBindings> Bindings { get; set; } = new();

  /// <summary>
  /// The names of all the message busses.
  /// </summary>
  public HashSet<string> BusNames { get; set; } = new();

  internal string? GetDefaultBus()
  {
    return DefaultBus is not null
      ? DefaultBus
      : BusNames.Count == 1
        ? BusNames.Single()
        : null;
  }
}
