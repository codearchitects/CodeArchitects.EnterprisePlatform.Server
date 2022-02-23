using CodeArchitects.Platform.Application.SignalR;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extension methods for <see cref="IEndpointRouteBuilder"/>.
/// </summary>
public static class ApplicationHubEndpointRouteBuilderExtensions
{
  /// <summary>
  /// Adds endpoints for the hubs configured with <see cref="ApplicationSignalRServerBuilderExtensions.AddHubs(ISignalRServerBuilder, Assembly[])"/>. 
  /// </summary>
  /// <param name="endpoints">The endpoint builder.</param>
  /// <param name="configureOptions">A callback to configure dispatcher options.</param>
  public static void MapHubs(this IEndpointRouteBuilder endpoints, Action<HttpConnectionDispatcherOptions>? configureOptions = null)
  {
    HubConfiguration configuration = endpoints.ServiceProvider.GetRequiredService<HubConfiguration>();

    foreach (KeyValuePair<Type, Type> entry in configuration.HubMap)
    {
      Type hubType = entry.Value;
      HubEndpointAttribute? hubEndpoint = hubType.GetCustomAttribute<HubEndpointAttribute>();
      if (hubEndpoint is null)
      {
        // TODO: Log warning
        continue;
      }

      MapHubDelegate.Create(hubType).Invoke(endpoints, hubEndpoint.Endpoint, configureOptions);
    }
  }

  internal abstract class MapHubDelegate
  {
    public abstract HubEndpointConventionBuilder Invoke(IEndpointRouteBuilder endpoints, string pattern, Action<HttpConnectionDispatcherOptions>? configureOptions);

    public static MapHubDelegate Create(Type hubType) => (MapHubDelegate)Activator.CreateInstance(typeof(ConcreteMapHubDelegate<>).MakeGenericType(hubType))!;

    private class ConcreteMapHubDelegate<THub> : MapHubDelegate
      where THub : Hub
    {
      public override HubEndpointConventionBuilder Invoke(IEndpointRouteBuilder endpoints, string pattern, Action<HttpConnectionDispatcherOptions>? configureOptions) => configureOptions is null
        ? endpoints.MapHub<THub>(pattern)
        : endpoints.MapHub<THub>(pattern, configureOptions);
    }
  }
}
