using CodeArchitects.Platform.Messaging.AspNetCore.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Messaging.AspNetCore;

/// <summary>
/// Routes the received message to the correct handler.
/// </summary>
internal class TopicRouter
{
  private readonly IDictionary<string, HandlerDelegate> _delegates;
  private readonly IMessageBiMap _messageMap;

  /// <summary>
  /// Creates a new <see cref="TopicRouter"/>.
  /// </summary>
  /// <param name="delegates">Associates message names to the respective handler delegate.</param>
  /// <param name="messageMap">Associates message names and message types.</param>
  public TopicRouter(IDictionary<string, HandlerDelegate> delegates, IMessageBiMap messageMap)
  {
    _delegates = delegates;
    _messageMap = messageMap;
  }

  public Task ExecuteAsync(HttpContext context)
  {
    using StreamReader streamReader = new StreamReader(context.Request.Body);
    using JsonTextReader jsonReader = new JsonTextReader(streamReader);
    ILogger? logger = context.RequestServices.TryGetLogger();

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
    if (!TryGetProperty(payloadObject, "type", out JToken? typeToken) || typeToken.Type is not JTokenType.String || typeToken.Value<string>() is not { } messageName)
    {
      logger?.LogError("CAEP mesagge envelope was missing 'type' property or it has wrong type.");
      context.Response.StatusCode = StatusCodes.Status400BadRequest;
      return;
    }
    if (!TryGetProperty(payloadObject, "message", out JToken? messageToken) || messageToken.Type is not JTokenType.Object)
    {
      logger?.LogError("CAEP mesagge envelope was missing 'message' property.");
      context.Response.StatusCode = StatusCodes.Status400BadRequest;
      return;
    }

    if (!_delegates.TryGetValue(messageName, out HandlerDelegate? @delegate))
    {
      if (!TryGetFallbackDelegate(messageName, out @delegate))
      {
        logger?.LogWarning($"No handler was registered for message of type '{messageName}'.");
        context.Response.StatusCode = StatusCodes.Status200OK;
        return;
      }
      _delegates.Add(messageName, @delegate);
    }

    try
    {
      await @delegate.HandleAsync(context, (JObject)messageToken);
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

  private bool TryGetFallbackDelegate(string messageName, [NotNullWhen(true)] out HandlerDelegate? @delegate)
  {
    if (!_messageMap.TryGetValue(messageName, out Type? messageType))
    {
      @delegate = null;
      return false;
    }

    messageType = messageType.BaseType;
    while (messageType is not null)
    {
      string baseMessageName = _messageMap[messageType];
      if (_delegates.TryGetValue(baseMessageName, out @delegate))
        return true;

      messageType = messageType.BaseType;
    }

    @delegate = null;
    return false;
  }

  private static bool TryGetProperty(JObject objectJson, string name, [NotNullWhen(true)] out JToken? property)
  {
    if (!objectJson.ContainsKey(name))
    {
      property = null;
      return false;
    }

    property = objectJson.Property(name)!.Value;
    return true;
  }
}
