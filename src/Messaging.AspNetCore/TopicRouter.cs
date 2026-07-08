using CodeArchitects.Platform.Messaging.AspNetCore.Handlers;
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
    using StreamReader streamReader = new(context.Request.Body);
    using JsonTextReader jsonReader = new(streamReader);
    ILogger logger = context.RequestServices.CreateMessagingLogger();

    switch (context.Request.ContentType)
    {
      case "application/cloudevents+json":
        return ProcessCloudEventAsync(context, jsonReader, logger);
      case "application/json":
        return ProcessRawPayloadAsync(context, jsonReader, logger);
      default:
        logger.LogError($"Unsupported media type: '{context.Request.ContentType}'.");
        context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
        return Task.CompletedTask;
    }
  }

  private async Task ProcessCloudEventAsync(HttpContext context, JsonReader jsonReader, ILogger logger)
  {
    JObject cloudEventObject;
    try
    {
      cloudEventObject = await JObject.LoadAsync(jsonReader);
    }
    catch (JsonReaderException ex)
    {
      logger.LogError(ex, "Could not deserialize message.");
      context.Response.StatusCode = StatusCodes.Status400BadRequest;
      return;
    }

    if (!TryGetString(cloudEventObject, "type", out string? type))
    {
      logger.LogError("Cloud event was missing 'type' property or it has wrong type.");
      context.Response.StatusCode = StatusCodes.Status400BadRequest;
      return;
    }

    bool hasData = cloudEventObject.ContainsKey("data");
    bool hasDataBase64 = cloudEventObject.ContainsKey("data_base64");
    if (hasData && hasDataBase64)
    {
      logger.LogError("Both 'data' and 'data_base64' properties were set in CloudEvent envelope.");
      context.Response.StatusCode = StatusCodes.Status400BadRequest;
      return;
    }

    if (hasData)
    {
      JToken dataToken = cloudEventObject.Property("data")!.Value;
      if (dataToken.Type is not JTokenType.Object)
      {
        // TODO: Extend support
        logger.LogError("The 'data' property of the CloudEvent envelope must be an object.");
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
      }

      await ExecuteAsync(context, (JObject)dataToken, type, logger);
      return;
    }
    
    if (hasDataBase64)
    {
      // TODO: Extend support
      logger.LogError("'data_base64' property of CloudEvent envelope is not supported yet.");
      context.Response.StatusCode = StatusCodes.Status500InternalServerError;
      return;
    }

    logger.LogError("Neither 'data' or 'data_base64' properties were set in CloudEvent envelope.");
    context.Response.StatusCode = StatusCodes.Status400BadRequest;
  }

  private async Task ProcessRawPayloadAsync(HttpContext context, JsonReader jsonReader, ILogger logger)
  {
    JObject messageObject;
    try
    {
      messageObject = await JObject.LoadAsync(jsonReader);
    }
    catch (JsonReaderException ex)
    {
      logger.LogError(ex, "Could not deserialize message.");
      context.Response.StatusCode = StatusCodes.Status400BadRequest;
      return;
    }

    if (!TryGetString(messageObject, "$type", out string? type))
    {
      logger.LogError("Message was missing '$type' property or it has wrong type.");
      context.Response.StatusCode = StatusCodes.Status400BadRequest;
      return;
    }

    await ExecuteAsync(context, messageObject, type, logger);
  }

  private async Task ExecuteAsync(HttpContext context, JObject payloadObject, string type, ILogger logger)
  {
    if (!_delegates.TryGetValue(type, out HandlerDelegate? @delegate))
    {
      if (!TryGetFallbackDelegate(type, out @delegate))
      {
        logger.LogInformation($"No handler was registered for message of type '{type}'.");
        context.Response.StatusCode = StatusCodes.Status200OK;
        return;
      }
      _delegates.Add(type, @delegate);
    }

    try
    {
      await @delegate.HandleAsync(context, payloadObject);
      context.Response.StatusCode = StatusCodes.Status200OK;
    }
    catch (InvalidMessageTypeException ex)
    {
      logger.LogError($"Cannot deserialize message of type '{type}' to type '{ex.MessageType.FullName}'.", ex);
      context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Error in executing a message handling action.");
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
      if (_messageMap.TryGetValue(messageType, out string? baseMessageName) && _delegates.TryGetValue(baseMessageName, out @delegate))
        return true;

      messageType = messageType.BaseType;
    }

    @delegate = null;
    return false;
  }

  private static bool TryGetString(JObject jObject, string name, [NotNullWhen(true)] out string? value)
  {
    JProperty? typeProperty = jObject.Property(name);

    if (
      typeProperty is null                       ||
      typeProperty.Value is not JValue typeValue ||
      typeValue.Value is not string str)
    {
      value = null;
      return false;
    }

    value = str;
    return true;
  }
}
