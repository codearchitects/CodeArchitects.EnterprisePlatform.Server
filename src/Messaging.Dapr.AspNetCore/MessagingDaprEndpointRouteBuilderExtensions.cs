using CodeArchitects.Platform.Dapr.AspNetCore.Services;
using CodeArchitects.Platform.Messaging.AspNetCore;
using CodeArchitects.Platform.Messaging.Dapr.AspNetCore;
using CodeArchitects.Platform.Messaging.Descriptors;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extension methods for adding messaging capabilities to the ASP.NET Core pipeline.
/// </summary>
public static class MessagingDaprEndpointRouteBuilderExtensions
{
  /// <summary>
  /// Adds endpoints for receiving messages. 
  /// </summary>
  /// <param name="endpoints">The endpoint builder.</param>
  public static void MapMessageHandlers(this IEndpointRouteBuilder endpoints)
  {
    if (endpoints is null)
      throw new ArgumentNullException(nameof(endpoints));
    if (endpoints.ServiceProvider.GetService<MessagingMarkerService>() is null)
      throw new InvalidOperationException($"Messaging has not been configured. Please chain a call to {nameof(MessagingDaprInfrastructureBuilderExtensions.AddMessaging)} to the Dapr infrastructure builder.");

    endpoints.MapSubscribeHandler();

    IDaprInfrastructureServiceProvider daprServices = endpoints.ServiceProvider.GetRequiredService<IDaprInfrastructureServiceProvider>();

    ITopicRouterFactory delegateFactory = endpoints.ServiceProvider.GetRequiredService<ITopicRouterFactory>();
    IMessagingDescriptor messagingDescriptor = daprServices.GetService<IMessagingDescriptor>();

    var handlerDescriptorGroups = messagingDescriptor.HandlerDescriptors.GroupBy(descriptor => (descriptor.Bus, descriptor.Topic));

    foreach (var handlerDescriptorGroup in handlerDescriptorGroups)
    {
      (string bus, string topic) = handlerDescriptorGroup.Key;
      TopicRouter router = delegateFactory.CreateRouter(handlerDescriptorGroup);

      endpoints
        .MapPost($"/pubsub/{bus}/{topic}", router.ExecuteAsync)
        .WithTopic(bus, topic);
    }
  }
}
