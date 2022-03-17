namespace CodeArchitects.Platform.Infrastructure.Dapr.Configuration;

internal static class DaprConfigurationExtensions
{
  public static string? GetDefaultBus(this DaprConfiguration configuration)
  {
    string? defaultBus = configuration.Service?.Messaging?.DefaultBus;
    if (defaultBus is not null)
      return defaultBus;

    ApplicationOptions? options = configuration.Application;
    if (options is null)
      return null;

    if (options.MessageBusses.Count == 1)
      return options.MessageBusses[0];

    return null;
  }

  public static string? GetDefaultStore(this DaprConfiguration configuration)
  {
    string? defaultStore = configuration.Service?.State?.DefaultStore;
    if (defaultStore is not null)
      return defaultStore;

    ApplicationOptions? options = configuration.Application;
    if (options is null)
      return null;

    if (options.StateStores.Count == 1)
      return options.StateStores[0];

    return null;
  }
}
