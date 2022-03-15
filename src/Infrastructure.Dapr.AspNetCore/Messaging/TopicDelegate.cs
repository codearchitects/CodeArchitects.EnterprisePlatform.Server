using CodeArchitects.Platform.Infrastructure.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Messaging;

internal class TopicDelegate
{
  private static readonly MethodInfo s_handleMessageNoResultMethod;
  private static readonly MethodInfo s_handleMessageWithResultMethod;

  static TopicDelegate()
  {
    s_handleMessageNoResultMethod = typeof(TopicDelegate).GetMethod(
      name: nameof(HandleMessageNoResult),
      bindingAttr: BindingFlags.Static | BindingFlags.NonPublic,
      binder: null,
      types: new Type[2] { typeof(HttpContext), typeof(JObject) },
      modifiers: null) ?? throw new MissingMethodException(nameof(TopicDelegate), nameof(HandleMessageNoResult));

    s_handleMessageWithResultMethod = typeof(TopicDelegate).GetMethod(
      name: nameof(HandleMessageWithResult),
      bindingAttr: BindingFlags.Static | BindingFlags.NonPublic,
      binder: null,
      types: new Type[2] { typeof(HttpContext), typeof(JObject) },
      modifiers: null) ?? throw new MissingMethodException(nameof(TopicDelegate), nameof(HandleMessageWithResult));
  }
  
  private readonly Dictionary<string, HandlerDelegate> _delegates;
  private readonly IReadOnlyDictionary<string, Type> _messageTypes;

  private TopicDelegate(Dictionary<string, HandlerDelegate> delegates, IReadOnlyDictionary<string, Type> messageTypes)
  {
    _delegates = delegates;
    _messageTypes = messageTypes;
  }

  public Task ExecuteAsync(HttpContext context)
  {
    using StreamReader streamReader = new StreamReader(context.Request.Body);
    using JsonTextReader jsonReader = new JsonTextReader(streamReader);
    ILogger? logger = context.RequestServices.GetService<ILoggerFactory>()?.CreateLogger("CAEP-Messaging");

    switch (context.Request.ContentType)
    {
      case "application/cloudevents+json":
        return ProcessCloudEvent(context, jsonReader, logger);
      case "application/json":
        return ProcessRawPayload(context, jsonReader, logger);
      default:
        logger?.LogError($"Unsupported media type: '{context.Request.ContentType}'.");
        context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
        return Task.CompletedTask;
    };
  }

  private async Task ProcessCloudEvent(HttpContext context, JsonReader jsonReader, ILogger? logger)
  {
    JObject cloudEventObject = await JObject.LoadAsync(jsonReader, context.RequestAborted);
    bool hasData = cloudEventObject.ContainsKey("data");
    bool hasDataBase64 = cloudEventObject.ContainsKey("data_base64");
    
    if (hasData && hasDataBase64)
    {
      logger?.LogError("Both 'data' and 'data_base64' properties were set in CloudEvent envelope.");
      context.Response.StatusCode = StatusCodes.Status400BadRequest;
      return;
    }

    if (hasData)
    {
      JToken dataToken = cloudEventObject.Property("data")!.Value;
      if (dataToken.Type is not JTokenType.Object)
      {
        // TODO: Extend support
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
      }

      await ExecuteAsync(context, (JObject)dataToken, logger);
    }
    else if (hasDataBase64)
    {
      // TODO: Extend support
      context.Response.StatusCode = StatusCodes.Status400BadRequest;
      return;
    }
    else
    {
      logger?.LogError("Neither 'data' or 'data_base64' properties were set in CloudEvent envelope.");
      context.Response.StatusCode = StatusCodes.Status400BadRequest;
      return;
    }
  }

  private async Task ProcessRawPayload(HttpContext context, JsonReader jsonReader, ILogger? logger)
  {
    JObject payloadObject = await JObject.LoadAsync(jsonReader, context.RequestAborted);
    await ExecuteAsync(context, payloadObject, logger);
  }

  private async Task ExecuteAsync(HttpContext context, JObject payloadObject, ILogger? logger)
  {
    bool hasType = payloadObject.ContainsKey("type");
    JToken typeToken = payloadObject.Property("type")!.Value;
    if (!hasType || typeToken.Type is not JTokenType.String || typeToken.Value<string>() is not { } messageName)
    {
      logger?.LogError("CAEP mesagge envelope was missing 'type' property.");
      context.Response.StatusCode = StatusCodes.Status400BadRequest;
      return;
    }

    bool hasMessage = payloadObject.ContainsKey("message");
    JToken messageToken = payloadObject.Property("message")!.Value;
    if (!hasMessage || messageToken.Type is not JTokenType.Object)
    {
      logger?.LogError("CAEP mesagge envelope was missing 'message' property.");
      context.Response.StatusCode = StatusCodes.Status400BadRequest;
      return;
    }

    if (!_delegates.TryGetValue(messageName, out HandlerDelegate? handlerDelegate))
    {
      if (!TryGetFallbackHandler(messageName, out handlerDelegate))
      {
        logger?.LogWarning($"No handler was registered for message of type '{messageName}'.");
        context.Response.StatusCode = StatusCodes.Status200OK;
        return;
      }
      _delegates.Add(messageName, handlerDelegate);
    }

    try
    {
      await handlerDelegate(context, (JObject)messageToken);
      context.Response.StatusCode = StatusCodes.Status200OK;
    }
    catch (InvalidMessageTypeException ex)
    {
      logger?.LogError($"Cannot deserialize message of type '{messageName}' to type '{ex.MessageType.FullName}'.", ex);
      context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
    catch (Exception ex)
    {
      logger?.LogError("Error in executing a message handling action.", ex);
      context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    }
  }

  private bool TryGetFallbackHandler(string messageName, [NotNullWhen(true)] out HandlerDelegate? handlerDelegate)
  {
    if (!_messageTypes.TryGetValue(messageName, out Type? messageType))
    {
      handlerDelegate = null;
      return false;
    }

    messageType = messageType.BaseType;
    while (messageType is not null)
    {
      string baseMessageName = messageType.GetCustomAttribute<MessageAttribute>()?.MessageName ?? messageType.Name;
      if (_delegates.TryGetValue(baseMessageName, out handlerDelegate))
      {
        return true;
      }

      messageType = messageType.BaseType;
    }

    handlerDelegate = null;
    return false;
  }

  public static TopicDelegate Create(IEnumerable<TopicDelegateData> topicData, ICollection<Type> messageTypes)
  {
    IReadOnlyDictionary<string, Type> messageTypeDictionary = messageTypes
      .ToDictionary(type => type.GetCustomAttribute<MessageAttribute>()?.MessageName ?? type.Name);

    Dictionary<string, HandlerDelegate> delegates = new Dictionary<string, HandlerDelegate>();
    foreach ((Type messageType, Type? resultType, Type handlerType) in topicData)
    {
      Debug.Assert(!messageType.IsGenericType, "Message types were supposed to be non-generic.");
      Debug.Assert(!messageType.IsValueType, "Message types were supposed to be reference types.");

      string messageName = messageType.GetCustomAttribute<MessageAttribute>()?.MessageName ?? messageType.Name;

      MethodInfo method = resultType is not null
        ? s_handleMessageWithResultMethod.MakeGenericMethod(messageType, resultType, handlerType)
        : s_handleMessageNoResultMethod.MakeGenericMethod(messageType, handlerType);
      HandlerDelegate @delegate = (HandlerDelegate)Delegate.CreateDelegate(typeof(HandlerDelegate), method);

      delegates.Add(messageName, @delegate); // This should not throw because of duplicate keys: it would mean we have duplicate MessageHandlerIdentities.
    }

    return new TopicDelegate(delegates, messageTypeDictionary);
  }

  private static async Task HandleMessageNoResult<TMessage, THandler>(HttpContext context, JObject messageObject)
    where TMessage : class
    where THandler : class, IMessageHandler<TMessage>
  {
    TMessage message = messageObject.ToObject<TMessage>() ?? throw new InvalidMessageTypeException(typeof(TMessage));

    IMessageHandler<TMessage> handler = context.RequestServices.GetRequiredService<THandler>();
    await handler.HandleAsync(message, context.RequestAborted);
    
    // TODO: Invoke bindings
  }

  private static async Task HandleMessageWithResult<TMessage, TResult, THandler>(HttpContext context, JObject messageObject)
    where TMessage : class
    where TResult : class
    where THandler : class, IMessageHandler<TMessage, TResult>
  {
    TMessage message = messageObject.ToObject<TMessage>() ?? throw new InvalidMessageTypeException(typeof(TMessage));

    IMessageHandler<TMessage, TResult> handler = context.RequestServices.GetRequiredService<THandler>();
    TResult result = await handler.HandleAsync(message, context.RequestAborted);
    
    // TODO: Invoke bindings
  }

  private delegate Task HandlerDelegate(HttpContext context, JObject messageJson);
}
