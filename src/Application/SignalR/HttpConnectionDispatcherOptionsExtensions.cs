using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Configuration;

namespace CodeArchitects.Platform.Application.SignalR
{
  /// <summary>
  /// Extension methods for <see cref="HttpConnectionDispatcherOptions"/>.
  /// </summary>
  public static class HttpConnectionDispatcherOptionsExtensions
  {
    /// <summary>
    /// Configures the transport type using the SignalR.Transport section of the application configuration.
    /// </summary>
    /// <param name="options">The dispatcher options.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <returns>The same options.</returns>
    public static HttpConnectionDispatcherOptions ConfigureTransports(this HttpConnectionDispatcherOptions options, IConfiguration configuration)
    {
      IConfigurationSection signalR = configuration.GetSection("SignalR");
      if (!signalR.Exists())
      {
        return options;
      }

      string[]? transports = signalR["Transports"]?.Split(",");
      if (transports is null || transports.Length == 0)
      {
        return options;
      }

      HttpTransportType transportTypes = HttpTransportType.None;
      foreach (string transport in transports)
      {
        switch (transport)
        {
          case nameof(HttpTransportType.WebSockets):
            transportTypes |= HttpTransportType.WebSockets;
            break;
          case nameof(HttpTransportType.ServerSentEvents):
            transportTypes |= HttpTransportType.ServerSentEvents;
            break;
          case nameof(HttpTransportType.LongPolling):
            transportTypes |= HttpTransportType.LongPolling;
            break;
          default:
            return options;
        }
      }

      options.Transports = transportTypes;
      return options;
    }
  }
}
