using CodeArchitects.Platform.Infrastructure.Dapr.Messaging;
using Moq;
using System;
using System.Reflection;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Tests
{
  public static class HandlerAssembly
  {
    public static class Valid
    {
      public static Type[] HandlerTypes => new Type[]
      {
        typeof(FakeMessage1Handler1), // Identities: (null, null, typeof(FakeMessage1))
        typeof(FakeMessage1HandlerWithTopic), // Identities: (null, TopicName, typeof(FakeMessage1))
        typeof(FakeMessage1HandlerWithBusAndTopic), // Identities: (BusName, TopicName, typeof(FakeMessage1))
        typeof(FakeMessage2Handler), // Identities: (null, null, typeof(FakeMessage2))
      };

      public static Assembly Instance
      {
        get
        {
          Mock<Assembly> assemblyMock = new Mock<Assembly>(behavior: MockBehavior.Strict);
          assemblyMock
            .Setup(x => x.GetTypes())
            .Returns(HandlerTypes);
          assemblyMock
            .Setup(x => x.GetHashCode())
            .Returns(nameof(Valid).GetHashCode());
          return assemblyMock.Object;
        }
      }

      public static Type HandlerForIdentity0 => typeof(FakeMessage1Handler1);

      public static Type HandlerForIdentity1 => typeof(FakeMessage1HandlerWithTopic);

      public static Type HandlerForIdentity2 => typeof(FakeMessage1HandlerWithBusAndTopic);

      public static Type HandlerForIdentity3 => typeof(FakeMessage2Handler);
    }

    public static class Invalid
    {
      public static Type[] HandlerTypes => new Type[]
      {
        typeof(FakeMessage1Handler1), // Identities: (null, null, typeof(FakeMessage1))
        typeof(FakeMessage1Handler2), // Identities: (null, null, typeof(FakeMessage1))
        typeof(FakeMessage1HandlerWithTopic), // Identities: (null, TopicName, typeof(FakeMessage1))
        typeof(FakeMessage1HandlerWithBusAndTopic), // Identities: (BusName, TopicName, typeof(FakeMessage1))
        typeof(FakeMessage2Handler), // Identities: (null, null, typeof(FakeMessage2))
        typeof(FakeMessage1And2Handler) // Identities: (null, null, typeof(FakeMessage1)), (null, null, typeof(FakeMessage2))
      };

      public static Assembly Instance
      {
        get
        {
          Mock<Assembly> assemblyMock = new Mock<Assembly>(behavior: MockBehavior.Strict);
          assemblyMock
            .Setup(x => x.GetTypes())
            .Returns(HandlerTypes);
          assemblyMock
            .Setup(x => x.GetHashCode())
            .Returns(nameof(Invalid).GetHashCode());
          return assemblyMock.Object;
        }
      }

      public static Type[] InvalidHandlerTypes => new Type[]
      {
        typeof(FakeMessage1Handler1),
        typeof(FakeMessage1Handler2),
        typeof(FakeMessage2Handler),
        typeof(FakeMessage1And2Handler)
      };
    }

    internal static MessageHandlerIdentity[] ExpectedIdentities => new MessageHandlerIdentity[]
    {
      new MessageHandlerIdentity(null, null, typeof(FakeMessage1)), // Handlers: FakeMessage1Handler1, FakeMessage1Handler2, FakeMessage1And2Handler
      new MessageHandlerIdentity(null, FakeMessage1HandlerWithTopic.TopicName, typeof(FakeMessage1)), // Handlers: FakeMessage1HandlerWithTopic
      new MessageHandlerIdentity(FakeMessage1HandlerWithBusAndTopic.BusName, FakeMessage1HandlerWithBusAndTopic.TopicName, typeof(FakeMessage1)), // Handlers: FakeMessage1HandlerWithBusAndTopic
      new MessageHandlerIdentity(null, null, typeof(FakeMessage2)), // Handlers: FakeMessage2Handler, FakeMessage1And2Handler
    };
  }
}
