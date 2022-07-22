using Castle.Components.DictionaryAdapter;
using CodeArchitects.Platform.Messaging.AspNetCore.Configuration;
using CodeArchitects.Platform.Messaging.Bindings;
using CodeArchitects.Platform.Messaging.Descriptors;
using CodeArchitects.Platform.Messaging.Descriptors.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OneOf;
using System.Reflection;

namespace CodeArchitects.Platform.Messaging.AspNetCore.Descriptors;

internal class ConfigurationDescriptorFactory
{
  private readonly IDictionaryAdapterFactory _adapterFactory;
  private readonly ILogger _logger;
  private readonly IReadOnlyDictionary<string, Type> _typeAliases;

  public ConfigurationDescriptorFactory(IDictionaryAdapterFactory adapterFactory, ILogger logger, IReadOnlyDictionary<string, Type> typeAliases)
  {
    _adapterFactory = adapterFactory;
    _logger = logger;
    _typeAliases = typeAliases;
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
    Type? handlerType = Type.GetType(handlerName);
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
    if (binding.MessageType is null)
    {
      _logger.LogWarning("'MessageType' was null.");
      return null;
    }
    Type? messageType = Type.GetType(binding.MessageType);
    if (messageType is null)
    {
      _logger.LogWarning("Could not find message type '{messageName}'.", binding.MessageType);
      return null;
    }

    Type? resultType = GetResultType(binding.ResultType, binding.ResultTypes);
    if (resultType is null)
      return null;

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
      _logger.LogWarning("No bus configured for handler {className}, message {messageName} and result {resultName}.", handlerType.FullName, binding.MessageType, binding.ResultType);
      return null;
    }

    string? topic = binding.Topic ?? classBinding.Topic ?? defaultTopic;
    if (topic is null)
    {
      _logger.LogWarning("No topic configured for handler {className}, message {messageName} and result {resultName}.", handlerType.FullName, binding.MessageType, binding.ResultType);
      return null;
    }

    IReadOnlyCollection<IOutputBindingDescriptor> bindingDescriptors = ProcessOutputBindings(binding.Output).ToList();
    return new HandlerDescriptor(bus, topic, messageType, resultType, handlerType, bindingDescriptors);

    Type? GetResultType(string? resultType, IReadOnlyList<string> resultTypes)
    {
      if (resultType is not null)
      {
        Type? result = Type.GetType(resultType);
        if (result is null)
        {
          _logger.LogWarning("Could not find result type '{resultName}'.", resultType);
        }

        return result;
      }

      if (resultTypes.Count == 0)
        return typeof(void);

      int arity = resultTypes.Count;

      if (arity == 1)
      {
        resultType = resultTypes[0];
        if (resultType is null)
        {
          _logger.LogWarning("'ResultTypes' section contained a null element.");
          return null;
        }

        Type? result = Type.GetType(resultType);
        if (result is null)
        {
          _logger.LogWarning("Could not find result type '{resultName}'.", resultType);
        }

        return result;
      }

      Type oneOfType = arity switch
      {
        2 => typeof(OneOf<,>),
        3 => typeof(OneOf<,,>),
        4 => typeof(OneOf<,,,>),
        5 => typeof(OneOf<,,,,>),
        6 => typeof(OneOf<,,,,,>),
        7 => typeof(OneOf<,,,,,,>),
        8 => typeof(OneOf<,,,,,,,>),
        9 => typeof(OneOf<,,,,,,,,>),
        _ => throw new NotSupportedException("Can support a maximum of 9 result types in a union type.")
      };

      Type[] genericArguments = new Type[arity];
      for (int i = 0; i < arity; i++)
      {
        resultType = resultTypes[i];
        if (resultType is null)
        {
          _logger.LogWarning("'ResultTypes' section contained a null element.");
          return null;
        }

        Type? typeArgument = Type.GetType(resultType);
        if (typeArgument is null)
        {
          _logger.LogWarning("Could not find result type '{resultName}'.", resultType);
          return null;
        }

        genericArguments[i] = typeArgument;
      }

      return oneOfType.MakeGenericType(genericArguments);
    }
  }

  private IEnumerable<IOutputBindingDescriptor> ProcessOutputBindings(IEnumerable<OutputBindingConfig> outputBindings)
  {
    foreach (OutputBindingConfig outputBinding in outputBindings)
    {
      string name = outputBinding.Name;
      if (name is null)
      {
        _logger.LogWarning("'Metadata:Name' was null.");
        continue;
      }

      Type? metadataType = _typeAliases.GetValueOrDefault(name) ?? Type.GetType(name);
      if (metadataType is null)
      {
        _logger.LogWarning("Could not find metadata type '{metadataName}'.", name);
        continue;
      }
      if (!metadataType.IsInterface)
      {
        _logger.LogWarning("Type '{metadataName}' is not an interface.", name);
        continue;
      }
      if (!typeof(IOutputMetadata).IsAssignableFrom(metadataType))
      {
        _logger.LogWarning("Type '{metadataName}' is not a metadata type.", name);
        continue;
      }

      BinderDictionary dictionary = CreateBinderDictionary(metadataType, outputBinding.Metadata);
      object metadataObject = _adapterFactory.GetAdapter(metadataType, dictionary);

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

  private static BinderDictionary CreateBinderDictionary(Type metadataType, Dictionary<string, IConfigurationSection> metadataDictionary)
  {
    Dictionary<string, PropertyInfo> properties = new();
    AddProperties(metadataType);

    return new BinderDictionary(metadataDictionary, properties);

    void AddProperties(Type type)
    {
      foreach (PropertyInfo property in type.GetProperties())
      {
        properties.Add(property.Name, property);
      }
      foreach (Type interfaceType in type.GetInterfaces())
      {
        AddProperties(interfaceType);
      }
    }
  }
}
