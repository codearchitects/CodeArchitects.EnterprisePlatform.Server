using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.Common.Internals;
using CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging;

/// <summary>
/// Implementation of <see cref="IMessagingConfiguration"/>.
/// </summary>
internal class MessagingConfiguration : IMessagingConfiguration
{
  private MessagingConfiguration(IReadOnlyDictionary<MessageHandlerIdentity, ImplementationPair> handlerMap, IReadOnlyCollection<Type> messageTypes)
  {
    HandlerMap = handlerMap;
    MessageTypes = messageTypes;
  }

  public IReadOnlyDictionary<MessageHandlerIdentity, ImplementationPair> HandlerMap { get; }

  public IReadOnlyCollection<Type> MessageTypes { get; }

  /// <summary>
  /// Scans the given assemblies for <see cref="IMessageHandler{TMessage}"/> implementations and creates an instance of <see cref="MessagingConfiguration"/>.
  /// </summary>
  /// <param name="assemblies">The assemblies to scan for handler types.</param>
  /// <param name="options">The messaging options.</param>
  /// <returns>An instance of <see cref="MessagingConfiguration"/>.</returns>
  public static MessagingConfiguration Create(IEnumerable<Assembly> assemblies, DaprMessagingOptions? options)
  {
    IEnumerable<Type> allTypes = assemblies.SelectMany(assembly => assembly.GetTypes());
    HashSet<Type> messageTypes = allTypes.Where(type => type.IsDefined(typeof(MessageAttribute))).ToHashSet();
    IEnumerable<Type> handlerConcreteTypes = allTypes.Where(type => !type.IsInterface && !type.IsAbstract && IsMessageHandler(type));

    IEnumerable<IGrouping<MessageHandlerIdentity, ImplementationPair>> identityMap = handlerConcreteTypes
      .SelectMany(concreteType => GetHandlerIdentities(concreteType, options).Select(pair => (pair.Identity, pair.InterfaceType, concreteType)))
      .GroupBy(x => x.Identity, x => new ImplementationPair(x.InterfaceType, x.concreteType));

    Dictionary<MessageHandlerIdentity, ImplementationPair> handlerMap = identityMap.ToDictionary(
      x => x.Key,
      x => GetSingleOrThrow(x));

    foreach (ImplementationPair pair in handlerMap.Values)
    {
      messageTypes.Add(pair.InterfaceType.GetGenericArguments()[0]);
    }

    return new MessagingConfiguration(handlerMap, messageTypes);
  }

  private static IEnumerable<(MessageHandlerIdentity Identity, Type InterfaceType)> GetHandlerIdentities(Type concreteType, DaprMessagingOptions? options)
  {
    IReadOnlyDictionary<string, DaprMessagingBindings>? allBindings = options?.Bindings;

    MessageHandlerAttribute? typeAttribute = concreteType.GetCustomAttribute<MessageHandlerAttribute>();
    return concreteType.GetGenericInterfaces(typeof(IMessageHandler<>), typeof(IMessageHandler<,>))
      .Select(interfaceType =>
      {
        string name = concreteType.Name;
        DaprMessagingBindings? bindings = allBindings is not null && allBindings.ContainsKey(name)
          ? allBindings[name]
          : null;

        InterfaceMapping interfaceMap = concreteType.GetInterfaceMap(interfaceType);
        Debug.Assert(interfaceMap.TargetMethods.Length == 1, $"The {nameof(IMessageHandler<object>)} interface was supposed to have a single method.");
        MethodInfo handleAsyncMethod = interfaceMap.TargetMethods[0];
        MessageHandlerAttribute? methodAttribute = handleAsyncMethod.GetCustomAttribute<MessageHandlerAttribute>();

        if (typeAttribute is null && methodAttribute is null)
          return default;

        Type messageType = interfaceType.GetGenericArguments()[0];
        if (messageType.IsGenericType)
          throw new NotSupportedException($"Generic message types are not suported. Message type was '{messageType.GetGenericTypeDefinition().FullName}'.");
        if (messageType.IsValueType)
          throw new NotSupportedException($"Message types must be reference types. Message type was '{messageType.FullName}'.");

        MessageHandlerIdentity identity = new MessageHandlerIdentity(
          BusName: methodAttribute?.BusName ?? typeAttribute?.BusName ?? bindings?.BusName ?? options?.GetDefaultBus(),
          Topic: methodAttribute?.Topic ?? typeAttribute?.Topic ?? bindings?.Topic ?? MessageBus.DefaultTopic,
          MessageType: messageType
        );

        return (identity, interfaceType);
      })
      .Where(pair => pair != default);
  }

  private static bool IsMessageHandler(Type type)
  {
    return type.ImplementsGenericInterface(typeof(IMessageHandler<>)) || type.ImplementsGenericInterface(typeof(IMessageHandler<,>));
  }

  private static ImplementationPair GetSingleOrThrow(IGrouping<MessageHandlerIdentity, ImplementationPair> grouping)
  {
    try
    {
      return grouping.Single();
    }
    catch (InvalidOperationException)
    {
      throw new ServiceRegistrationException(
        message: "Attempt to register more than one handler of the same message type, on the same message bus and topic.",
        implementationTypes: grouping.Select(pair => pair.ImplementationType).ToArray());
    }
  }
}
