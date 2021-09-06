using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging
{
  /// <summary>
  /// Implementation of <see cref="IMessageHandlerConfiguration"/>.
  /// </summary>
  internal class MessageHandlerConfiguration : IMessageHandlerConfiguration
  {
    private readonly Dictionary<MessageHandlerIdentity, Type> _handlerMap;

    /// <summary>
    /// Creates an instance of <see cref="MessageHandlerConfiguration"/>.
    /// Scans the given assemblies for <see cref="IMessageHandler{TMessage}"/> implementations.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan for handler types.</param>
    public MessageHandlerConfiguration(IEnumerable<Assembly> assemblies)
    {
      _handlerMap = CreateHandlerMap(assemblies);
    }

    public IReadOnlyDictionary<MessageHandlerIdentity, Type> HandlerMap => _handlerMap;

    private static Dictionary<MessageHandlerIdentity, Type> CreateHandlerMap(IEnumerable<Assembly> assemblies)
    {
      IEnumerable<Type> handlerConcreteTypes = assemblies
        .SelectMany(x => x.GetTypes())
        .Where(x =>
          !x.IsInterface &&
          !x.IsAbstract &&
          x.ImplementsGenericInterface(typeof(IMessageHandler<>)));

      IEnumerable<IGrouping<MessageHandlerIdentity, Type>> identityMap = handlerConcreteTypes
        .SelectMany(concreteType => concreteType
          .GetHandlerIdentities()
          .Select(identity => (identity, concreteType)))
        .GroupBy(x => x.identity, x => x.concreteType);

      if (identityMap.Any(x => x.Count() > 1))
      {
        IEnumerable<Type> invalidTypes = identityMap.Where(x => x.Count() > 1).SelectMany(x => x).Distinct();
        throw new ServiceRegistrationException("Some message handlers handle the same message type on the same bus and topic.", invalidTypes);
      }

      return identityMap.ToDictionary(
        x => x.Key,
        x => x.Single());
    }
  }

  internal static class TypeExtensions
  {
    public static IEnumerable<MessageHandlerIdentity> GetHandlerIdentities(this Type concreteType)
    {
      return concreteType.GetGenericInterfaces(typeof(IMessageHandler<>))
        .Select(interfaceType => new MessageHandlerIdentity(
          busName:     concreteType.GetCustomAttribute<SubscribeToBusAttribute>()?.BusName,
          topic:       concreteType.GetCustomAttribute<SubscribeToTopicAttribute>()?.Topic,
          messageType: interfaceType.GetGenericArguments()[0]
        ));
    }
  }
}
