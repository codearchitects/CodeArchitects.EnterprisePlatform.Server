using Castle.Components.DictionaryAdapter;
using CodeArchitects.Platform.Messaging.AspNetCore.Configuration;
using CodeArchitects.Platform.Messaging.Bindings;
using CodeArchitects.Platform.Messaging.Descriptors;
using CodeArchitects.Platform.Messaging.Descriptors.Implementation;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace CodeArchitects.Platform.Messaging.AspNetCore.Descriptors;

internal class ConfigurationDescriptorFactory
{
  private readonly IDictionaryAdapterFactory _adapterFactory;
  private readonly ILogger _logger;

  public ConfigurationDescriptorFactory(IDictionaryAdapterFactory adapterFactory, ILogger logger)
  {
    _adapterFactory = adapterFactory;
    _logger = logger;
  }

  public IMessagingDescriptor Create(IReadOnlyDictionary<string, HandlerClassBindingsConfig> bindings, string? defaultBus, string? defaultTopic)
  {
    IEnumerable<IHandlerDescriptor> handlerDescriptors = CreateHandlerDescriptors(bindings, defaultBus, defaultTopic);
    IEnumerable<IMessageDescriptor> messageDescriptors = CreateMessageDescriptors(handlerDescriptors);

    return new MessagingDescriptor(handlerDescriptors, messageDescriptors);
  }

  private IEnumerable<IHandlerDescriptor> CreateHandlerDescriptors(IReadOnlyDictionary<string, HandlerClassBindingsConfig> bindings, string? defaultBus, string? defaultTopic)
  {
    List<IHandlerDescriptor> handlerDescriptors = new();
    foreach ((string handlerName, HandlerClassBindingsConfig classBinding) in bindings)
    {
      handlerDescriptors.AddRange(ProcessClassBindings(handlerName, classBinding, defaultBus, defaultTopic));
    }

    return handlerDescriptors;
  }

  private IEnumerable<IHandlerDescriptor> ProcessClassBindings(string handlerName, HandlerClassBindingsConfig classBinding, string? defaultBus, string? defaultTopic)
  {
    Type? handlerType = GetType(handlerName);
    if (handlerType is null)
    {
      _logger.LogWarning("Could not find handler type '{className}'.", handlerName);
      yield break;
    }
    if (handlerType.IsGenericType)
    {
      _logger.LogWarning("Handler type '{className}' is a generic type, which is not allowed.", handlerName);
      yield break;
    }

    foreach (HandlerBindingsConfig binding in classBinding.Methods)
    {
      IHandlerDescriptor? handlerDescriptor = ProcessMethodBinding(handlerType, classBinding, binding, defaultBus, defaultTopic);
      if (handlerDescriptor is null)
        continue;

      yield return handlerDescriptor;
    }
  }

  private IHandlerDescriptor? ProcessMethodBinding(Type handlerType, HandlerClassBindingsConfig classBinding, HandlerBindingsConfig binding, string? defaultBus, string? defaultTopic)
  {
    Type? messageType = GetType(binding.MessageName);
    if (messageType is null)
    {
      _logger.LogWarning("Could not find message type '{messageName}'.", binding.MessageName);
      return null;
    }

    Type? resultType = binding.ResultName is null ? typeof(void) : GetType(binding.ResultName);
    if (resultType is null)
    {
      _logger.LogWarning("Could not find result type '{resultName}'.", binding.ResultName);
      return null;
    }

    Type interfaceType = resultType == typeof(void)
      ? typeof(IMessageHandler<>).MakeGenericType(messageType)
      : typeof(IMessageHandler<,>).MakeGenericType(messageType, resultType);
    if (!interfaceType.IsAssignableFrom(handlerType))
    {
      _logger.LogWarning("Type '{className}' is not assignable to {interfaceName}.", handlerType.FullName, interfaceType.FullName);
      return null;
    }

    string? bus = binding.Bus ?? classBinding.Bus ?? defaultBus;
    if (bus is null)
    {
      _logger.LogWarning("No bus configured for handler {className}, message {messageName} and result {resultName}.", handlerType.FullName, binding.MessageName, binding.ResultName);
      return null;
    }

    string? topic = binding.Topic ?? classBinding.Topic ?? defaultTopic;
    if (topic is null)
    {
      _logger.LogWarning("No topic configured for handler {className}, message {messageName} and result {resultName}.", handlerType.FullName, binding.MessageName, binding.ResultName);
      return null;
    }

    IReadOnlyCollection<IOutputBindingDescriptor> bindingDescriptors = ProcessOutputBindings(binding.Output).ToList();
    return new HandlerDescriptor(bus, topic, interfaceType, handlerType, bindingDescriptors);
  }

  private IEnumerable<IOutputBindingDescriptor> ProcessOutputBindings(IEnumerable<OutputBindingConfig> outputBindings)
  {
    foreach (OutputBindingConfig outputBinding in outputBindings)
    {
      Type? metadataType = GetType(outputBinding.Name);
      if (metadataType is null)
      {
        _logger.LogWarning("Could not find metadata type '{metadataName}'.", outputBinding.Name);
        continue;
      }
      if (!typeof(IOutputMetadata).IsAssignableFrom(metadataType))
      {
        _logger.LogWarning("Type '{metadataName}' is not a metadata type.", outputBinding.Name);
        continue;
      }

      object metadataObject = _adapterFactory.GetAdapter(metadataType, outputBinding.Metadata); // TODO: Test what happens when dictionary is invalid

      yield return new OutputBindingDescriptor(metadataType, metadataObject);
    }
  }

  private IEnumerable<IMessageDescriptor> CreateMessageDescriptors(IEnumerable<IHandlerDescriptor> handlerDescriptors)
  {
    Dictionary<Type, IMessageDescriptor> messageDescriptors = new();
    foreach (IHandlerDescriptor handlerDescriptor in handlerDescriptors)
    {
      Type messageType = handlerDescriptor.MessageType;
      if (!messageDescriptors.ContainsKey(messageType))
      {
        messageDescriptors.Add(messageType, MessageDescriptor.Create(messageType));
      }
    }

    return messageDescriptors.Values;
  }

  private static Type? GetType(string fullyQualifiedName)
  {
    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
    {
      if (assembly.GetType(fullyQualifiedName) is Type type)
        return type;
    }

    return null;
  }
}
