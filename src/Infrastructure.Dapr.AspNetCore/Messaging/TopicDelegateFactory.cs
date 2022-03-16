using CodeArchitects.Platform.Common.Internals;
using CodeArchitects.Platform.Infrastructure.Dapr.Messaging;
using CodeArchitects.Platform.Infrastructure.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Messaging;

internal class TopicDelegateFactory : ITopicDelegateFactory
{
  private static readonly MethodInfo s_handleMessageNoResultMethod;
  private static readonly MethodInfo s_handleMessageWithResultMethod;

  static TopicDelegateFactory()
  {
    s_handleMessageNoResultMethod = typeof(TopicDelegateFactory).GetMethod(
      name: nameof(HandleMessageNoResult),
      bindingAttr: BindingFlags.Static | BindingFlags.NonPublic,
      binder: null,
      types: new Type[2] { typeof(HttpContext), typeof(JObject) },
      modifiers: null) ?? throw new MissingMethodException(nameof(TopicDelegateFactory), nameof(HandleMessageNoResult));

    s_handleMessageWithResultMethod = typeof(TopicDelegateFactory).GetMethod(
      name: nameof(HandleMessageWithResult),
      bindingAttr: BindingFlags.Static | BindingFlags.NonPublic,
      binder: null,
      types: new Type[2] { typeof(HttpContext), typeof(JObject) },
      modifiers: null) ?? throw new MissingMethodException(nameof(TopicDelegateFactory), nameof(HandleMessageWithResult));
  }

  private readonly IReadOnlyDictionary<MessageHandlerIdentity, ImplementationPair> _handlerMap;
  private readonly IReadOnlyDictionary<string, Type> _messageTypes;

  public TopicDelegateFactory(IReadOnlyDictionary<MessageHandlerIdentity, ImplementationPair> handlerMap, IReadOnlyDictionary<string, Type> messageTypes)
  {
    _handlerMap = handlerMap;
    _messageTypes = messageTypes;
  }

  public static TopicDelegateFactory Create(IMessagingConfiguration configuration)
  {
    Dictionary<string, Type> messageTypes = configuration.MessageTypes.ToDictionary(type => type.GetCustomAttribute<MessageAttribute>()?.MessageName ?? type.Name);
    return new TopicDelegateFactory(configuration.HandlerMap, messageTypes);
  }

  public TopicDelegate CreateDelegate(IEnumerable<MessageHandlerIdentity> identities)
  {
    Dictionary<string, HandlerDelegate> delegates = new Dictionary<string, HandlerDelegate>();
    foreach (MessageHandlerIdentity identity in identities)
    {
      ImplementationPair pair = _handlerMap[identity];
      Type handlerType = pair.ImplementationType;
      Type messageType = identity.MessageType;
      Type[] interfaceArguments = pair.InterfaceType.GetGenericArguments();
      Type? resultType = interfaceArguments.Length == 2
        ? interfaceArguments[1]
        : null;

      Debug.Assert(!messageType.IsGenericType, "Message types were supposed to be non-generic.");
      Debug.Assert(!messageType.IsValueType, "Message types were supposed to be reference types.");

      string messageName = messageType.GetCustomAttribute<MessageAttribute>()?.MessageName ?? messageType.Name;

      MethodInfo method = resultType is null
        ? s_handleMessageNoResultMethod.MakeGenericMethod(messageType, handlerType)
        : s_handleMessageWithResultMethod.MakeGenericMethod(messageType, resultType, handlerType);
      HandlerDelegate @delegate = (HandlerDelegate)Delegate.CreateDelegate(typeof(HandlerDelegate), method);

      // TODO: Chain output binding

      delegates.Add(messageName, @delegate); // This should not throw because of duplicate keys: it would mean we have duplicate MessageHandlerIdentities.
    }

    return new TopicDelegate(delegates, _messageTypes);
  }

  private static async Task<object?> HandleMessageNoResult<TMessage, THandler>(HttpContext context, JObject messageObject)
    where TMessage : class
    where THandler : class, IMessageHandler<TMessage>
  {
    TMessage message = messageObject.ToObject<TMessage>() ?? throw new InvalidMessageTypeException(typeof(TMessage));

    IMessageHandler<TMessage> handler = context.RequestServices.GetRequiredService<THandler>();
    await handler.HandleAsync(message, context.RequestAborted);
    return null;
  }

  private static async Task<object?> HandleMessageWithResult<TMessage, TResult, THandler>(HttpContext context, JObject messageObject)
    where TMessage : class
    where TResult : class
    where THandler : class, IMessageHandler<TMessage, TResult>
  {
    TMessage message = messageObject.ToObject<TMessage>() ?? throw new InvalidMessageTypeException(typeof(TMessage));

    IMessageHandler<TMessage, TResult> handler = context.RequestServices.GetRequiredService<THandler>();
    TResult result = await handler.HandleAsync(message, context.RequestAborted);
    return result;
  }
}
