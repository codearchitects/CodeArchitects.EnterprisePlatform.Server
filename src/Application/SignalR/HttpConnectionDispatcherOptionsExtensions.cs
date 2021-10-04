using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Configuration;

namespace CodeArchitects.Platform.Application.SignalR
{
  public static class HttpConnectionDispatcherOptionsExtensions
  {
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
