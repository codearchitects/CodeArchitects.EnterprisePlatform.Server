using CodeArchitects.Platform.Messaging.AspNetCore.Configuration;
using CodeArchitects.Platform.Messaging.Bindings;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CodeArchitects.Platform.Messaging.Dapr.AspNetCore;

internal class DaprMessagingOptionsBuilder : IDaprMessagingOptionsBuilder
{
  private readonly MessagingConfig _config;
  private readonly HashSet<Type> _handlerTypes;
  private readonly HashSet<Type> _messageTypes;
  private readonly Dictionary<string, Type> _typeAliases;
  private readonly List<ServiceDescriptor> _bindingServiceDescriptors;

  public DaprMessagingOptionsBuilder(MessagingConfig config)
  {
    _config = config;
    _handlerTypes = new();
    _messageTypes = new();
    _typeAliases = new();
    _bindingServiceDescriptors = new();
  }

  public IEnumerable<Type> HandlerTypes => _handlerTypes;

  public IEnumerable<Type> MessageTypes => _messageTypes;

  public IReadOnlyDictionary<string, Type> TypeAliases => _typeAliases;

  public IEnumerable<ServiceDescriptor> BindingServiceDescriptors => _bindingServiceDescriptors;

  public IDaprMessagingOptionsBuilder Configure(Action<MessagingConfig> configure)
  {
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));

    configure(_config);
    return this;
  }

  public IDaprMessagingOptionsBuilder AddHandler(Type handlerType)
  {
    if (handlerType is null)
      throw new ArgumentNullException(nameof(handlerType));
    if (!handlerType.ImplementsGenericInterface(typeof(IMessageHandler<>)) && !handlerType.ImplementsGenericInterface(typeof(IMessageHandler<,>)))
      throw new ArgumentException($"'{nameof(handlerType)}' must be a type implementing the IMessageHandler interface", nameof(handlerType));

    _handlerTypes.Add(handlerType);

    return this;
  }

  public IDaprMessagingOptionsBuilder AddMessage(Type messageType)
  {
    if (messageType is null)
      throw new ArgumentNullException(nameof(messageType));

    _messageTypes.Add(messageType);
    return this;
  }

  public IDaprMessagingOptionsBuilder ScanAssembly(Assembly assembly)
  {
    if (assembly is null)
      throw new ArgumentNullException(nameof(assembly));

    foreach (Type type in assembly.GetTypes())
    {
      if (type.IsDefined(typeof(MessageHandlerAttribute)))
      {
        AddHandler(type);
        continue;
      }

      if (type.IsDefined(typeof(MessageAttribute)))
      {
        AddMessage(type);
        continue;
      }
    }

    return this;
  }

  public IDaprMessagingOptionsBuilder RegisterOutputBinding(Type outputBindingType, ServiceLifetime lifetime = ServiceLifetime.Scoped)
  {
    IEnumerable<Type> interfaceTypes = outputBindingType
      .GetInterfaces()
      .Where(type => typeof(IOutputMetadata).IsAssignableFrom(type) && type != typeof(IOutputMetadata) && type != typeof(ITypedOutputMetadata));

    if (!interfaceTypes.Any())
      throw new ArgumentException($"'{nameof(outputBindingType)}' must implement the '{typeof(IOutputBinding<>)}' interface.");

    foreach (Type interfaceType in interfaceTypes)
    {
      _bindingServiceDescriptors.Add(new ServiceDescriptor(interfaceType, outputBindingType, lifetime));
    }

    return this;
  }

  public IDaprMessagingOptionsBuilder RegisterOutputMetadataAlias(string alias, Type metadataType)
  {
    if (!typeof(IOutputMetadata).IsAssignableFrom(metadataType))
      throw new ArgumentException($"'{nameof(metadataType)}' should implement the IOutputMetadata interface.", nameof(metadataType));

    _typeAliases.Add("$" + alias, metadataType);
    return this;
  }
}
