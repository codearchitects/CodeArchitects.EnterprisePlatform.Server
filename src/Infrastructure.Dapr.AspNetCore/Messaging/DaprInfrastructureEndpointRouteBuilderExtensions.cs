using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Messaging;
using CodeArchitects.Platform.Infrastructure.Dapr.Messaging;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNetCore.Builder;

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
    if (endpoints is null)
      throw new ArgumentNullException(nameof(endpoints));

    if (endpoints.ServiceProvider.GetService<MessageHandlerMarkerService>() is null)
      throw new InvalidOperationException($"Message handlers have not been configured. Please chain a call to {nameof(DaprInfrastructureBuilderExtensions.AddMessageHandlers)} to the Dapr infrastructure builder.");

    IMessagingConfiguration messagingConfiguration = endpoints.ServiceProvider.GetRequiredService<IMessagingConfiguration>();
    ITopicDelegateFactory delegateFactory = endpoints.ServiceProvider.GetRequiredService<ITopicDelegateFactory>();

    ILoggerFactory? loggerFactory = endpoints.ServiceProvider.GetService<ILoggerFactory>();
    ILogger? logger = loggerFactory?.CreateLogger(Constants.LoggingCategory);

    endpoints.MapSubscribeHandler();

    IEnumerable<IGrouping<(string? BusName, string Topic), MessageHandlerIdentity>> identityGroups = messagingConfiguration.HandlerMap.Keys
      .GroupBy(identity => (identity.BusName, identity.Topic));

    foreach (var identityGroup in identityGroups)
    {
      (string? busName, string topic) = identityGroup.Key;
      if (busName is null)
      {
        logger?.LogWarning("Some handlers on topic '{0}' did not specified a message bus name, thus will be inactive.", topic);
        continue;
      }

      TopicDelegate @delegate = delegateFactory.CreateDelegate(identityGroup);

      endpoints
        .MapPost($"/pubsub/{busName}/{topic}", @delegate.ExecuteAsync)
        .WithTopic(busName, topic);
    }
  }
}
