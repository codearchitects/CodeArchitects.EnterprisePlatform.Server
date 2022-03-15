using CodeArchitects.Platform.Infrastructure.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Messaging;

internal class TopicDelegate
{
  private readonly Dictionary<string, HandlerDelegate> _delegates;
  private readonly IReadOnlyDictionary<string, Type> _messageTypes;

  public TopicDelegate(Dictionary<string, HandlerDelegate> delegates, IReadOnlyDictionary<string, Type> messageTypes)
  {
    _delegates = delegates;
    _messageTypes = messageTypes;
  }

  public Task ExecuteAsync(HttpContext context)
  {
    using StreamReader streamReader = new StreamReader(context.Request.Body);
    using JsonTextReader jsonReader = new JsonTextReader(streamReader);
    ILogger? logger = context.RequestServices.GetService<ILoggerFactory>()?.CreateLogger(Constants.LoggingCategory);

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
    catch
    {
      logger?.LogError("Error in executing a message handling action.");
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
}
