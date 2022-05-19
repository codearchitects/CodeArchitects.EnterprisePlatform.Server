using System.Collections.Generic;
using System.Linq;

namespace CodeArchitects.Platform.Infrastructure.Dapr.State;

/// <summary>
/// Provides state configuration options.
/// </summary>
public class DaprStateOptions
{
  /// <summary>
  /// The default state store name.
  /// </summary>
  public string? DefaultStore { get; set; }

  /// <summary>
  /// The names of all the state stores.
  /// </summary>
  public HashSet<string> StoreNames { get; set; } = new();

  internal string? GetDefaultStore()
  {
    return DefaultStore is not null
      ? DefaultStore
      : StoreNames.Count == 1
        ? StoreNames.Single()
        : null;
  }
}
