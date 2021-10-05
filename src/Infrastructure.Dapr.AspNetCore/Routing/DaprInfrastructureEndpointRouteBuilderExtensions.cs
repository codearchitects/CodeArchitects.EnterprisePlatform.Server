using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Routing;
using CodeArchitects.Platform.Infrastructure.Dapr.Configuration;
using CodeArchitects.Platform.Infrastructure.Dapr.Messaging;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;

namespace Microsoft.AspNetCore.Builder
{
  /// <summary>
  /// Extension methods for <see cref="IEndpointRouteBuilder"/>.
  /// </summary>
  public static class DaprInfrastructureEndpointRouteBuilderExtensions
  {
    /// <summary>
    /// Adds endpoints for the handlers configured with <see cref="DaprInfrastructureBuilderExtensions.AddMessageHandlers"/>. 
    /// </summary>
    /// <param name="endpoints">The endpoint builder.</param>
    public static void MapMessageHandlers(this IEndpointRouteBuilder endpoints)
    {
      if (endpoints is null) throw new ArgumentNullException(nameof(endpoints));

      IMessageHandlerConfiguration? handlerConfiguration = endpoints.ServiceProvider.GetService<IMessageHandlerConfiguration>();
      if (handlerConfiguration is null)
      {
        throw new InvalidOperationException($"Message handlers have not been configured. Please chain a call to {nameof(DaprInfrastructureBuilderExtensions.AddMessageHandlers)} to the Dapr infrastructure builder.");
      }

      endpoints.MapSubscribeHandler();

      DaprConfiguration? configuration = endpoints.ServiceProvider.GetService<DaprConfiguration>();
      string? defaultBus = configuration?.Service?.Messaging?.DefaultBus;

      JsonSerializerOptions serializerOptions = new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
      };

      foreach (MessageHandlerIdentity identity in handlerConfiguration.HandlerMap.Keys)
      {
        string? busName = identity.BusName ?? defaultBus;
        if (busName is null) // TODO: Log warning
          continue;

        MessageRequestDelegate requestDelegate = MessageRequestDelegate.Create(identity.MessageType, handlerConfiguration.HandlerMap[identity], serializerOptions);

        endpoints.MapPost($"/pubsub/{busName}/{identity.DaprTopic}", requestDelegate.ExecuteAsync)
                 .WithTopic(busName, identity.DaprTopic);
      }
    }
  }
}
