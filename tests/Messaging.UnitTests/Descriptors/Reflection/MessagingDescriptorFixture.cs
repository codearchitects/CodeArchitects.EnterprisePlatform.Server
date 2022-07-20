using CodeArchitects.Platform.Messaging.Fixtures;
using CodeArchitects.Platform.Messaging.Fixtures.DescriptorBuilder;
using Moq;
using System.Reflection;
using Xunit.Sdk;

namespace CodeArchitects.Platform.Messaging.Descriptors.Implementation;

public static class MessagingDescriptorFixture
{
  internal class StandardMessageHandlerDataAttribute : DataAttribute
  {
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      Type[] messageTypes = new[] { typeof(Message4), typeof(Message5) };
      Type[] concreteTypes;
      MessagingDescriptorBuilder builder;

      // With defaultBus != null and defaultTopic != null
      string defaultBus = "defaultBus";
      string defaultTopic = "defaultTopic";
      concreteTypes = new[] { typeof(StandardMessageHandler) };
      builder = new(MockBehavior.Strict);
      AddMessageDescriptors(builder);
      AddStandardMessageHandler1(builder);
      AddStandardMessageHandler2(builder);

      yield return new object?[] { concreteTypes, messageTypes, defaultBus, defaultTopic, builder.Descriptor };
    }

    private static void AddStandardMessageHandler1(IMessagingDescriptorBuilder builder)
    {
      builder
        .AddHandlerDescriptor(_ => _
          .SetBus(StandardMessageHandlerInfo.Identity1Bus)
          .SetTopic(StandardMessageHandlerInfo.Identity1Topic)
          .SetInterfaceType(typeof(IMessageHandler<Message1>))
          .SetConcreteType(typeof(StandardMessageHandler))
          .SetMessageType(typeof(Message1))
          .SetResultType(typeof(void)));
    }

    private static void AddStandardMessageHandler2(IMessagingDescriptorBuilder builder)
    {
      builder
        .AddHandlerDescriptor(_ => _
          .SetBus(StandardMessageHandlerInfo.ClassBus)
          .SetTopic(StandardMessageHandlerInfo.ClassTopic)
          .SetInterfaceType(typeof(IMessageHandler<Message2, Message1>))
          .SetConcreteType(typeof(StandardMessageHandler))
          .SetMessageType(typeof(Message2))
          .SetResultType(typeof(Message1))
          .AddOutputBindingDescriptor(_ => _
            .SetMetadataType(typeof(IFakeOutputMetadata1))
            .SetMetadataObject(StandardMessageHandlerInfo.Identity2MetadataObject)))
        .AddMessageDescriptor(_ => _
          .SetName(typeof(Message2).Name)
          .SetType(typeof(Message2)));
    }

    private static void AddMessageDescriptors(IMessagingDescriptorBuilder builder)
    {
      builder
        .AddMessageDescriptor(_ => _
          .SetName(Message4.MessageName)
          .SetType(typeof(Message4)))
        .AddMessageDescriptor(_ => _
          .SetName(Message1.MessageName)
          .SetType(typeof(Message1)))
        .AddMessageDescriptor(_ => _
          .SetName(typeof(Message5).Name)
          .SetType(typeof(Message5)))
        .AddMessageDescriptor(_ => _
          .SetName(typeof(Message3).Name)
          .SetType(typeof(Message3)));
    }
  }

  internal class MergeDataAttribute : DataAttribute
  {
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      MessagingDescriptorBuilder builder;
      IMessagingDescriptor first;
      IMessagingDescriptor second;
      IMessagingDescriptor expected;

      Type messageType1 = new MockType(1);
      Type messageType2 = new MockType(2);
      Type messageType3 = new MockType(3);

      Type resultType1 = new MockType(10);
      Type resultType2 = new MockType(11);
      Type resultType3 = new MockType(12);

      Type handlerType1 = new MockType(20);
      Type handlerType2 = new MockType(21);
      Type handlerType3 = new MockType(22);

      Type interfaceType1 = typeof(IMessageHandler<,>).MakeGenericType(messageType1, resultType1);
      Type interfaceType2 = typeof(IMessageHandler<,>).MakeGenericType(messageType2, resultType2);
      Type interfaceType3 = typeof(IMessageHandler<,>).MakeGenericType(messageType3, resultType3);

      Type metadataType1 = new MockType(30);
      Type metadataType2 = new MockType(31);
      Type metadataType3 = new MockType(32);

      object metadataObject1 = "metadata1";
      object metadataObject2 = "metadata2";
      object metadataObject3 = "metadata3";

      string bus1 = nameof(bus1);
      string bus2 = nameof(bus2);
      string bus3 = nameof(bus3);

      string topic1 = nameof(topic1);
      string topic2 = nameof(topic2);
      string topic3 = nameof(topic3);

      // Two sets of different messages
      builder = new(MockBehavior.Strict);
      builder
        .AddMessageDescriptor(_ => _
          .SetType(messageType1)
          .SetName(nameof(messageType1)));
      first = builder.Descriptor;

      builder = new(MockBehavior.Strict);
      builder
        .AddMessageDescriptor(_ => _
          .SetType(messageType2)
          .SetName(nameof(messageType2)));
      second = builder.Descriptor;

      builder = new(MockBehavior.Strict);
      builder
        .AddMessageDescriptor(_ => _
          .SetType(messageType1)
          .SetName(nameof(messageType1)))
        .AddMessageDescriptor(_ => _
          .SetType(messageType2)
          .SetName(nameof(messageType2)));
      expected = builder.Descriptor;

      yield return new object?[] { first, second, expected };

      // Two sets with one common message
      builder = new(MockBehavior.Strict);
      builder
        .AddMessageDescriptor(_ => _
          .SetType(messageType1)
          .SetName(nameof(messageType1)))
        .AddMessageDescriptor(_ => _
          .SetType(messageType2)
          .SetName(nameof(messageType2)));
      first = builder.Descriptor;

      builder = new(MockBehavior.Strict);
      builder
        .AddMessageDescriptor(_ => _
          .SetType(messageType2)
          .SetName(nameof(messageType2)))
        .AddMessageDescriptor(_ => _
          .SetType(messageType3)
          .SetName(nameof(messageType3)));
      second = builder.Descriptor;

      builder = new(MockBehavior.Strict);
      builder
        .AddMessageDescriptor(_ => _
          .SetType(messageType1)
          .SetName(nameof(messageType1)))
        .AddMessageDescriptor(_ => _
          .SetType(messageType2)
          .SetName(nameof(messageType2)))
        .AddMessageDescriptor(_ => _
          .SetType(messageType3)
          .SetName(nameof(messageType3)));
      expected = builder.Descriptor;

      yield return new object?[] { first, second, expected };

      // Two sets of different handlers
      builder = new(MockBehavior.Strict);
      builder
        .AddHandlerDescriptor(_ => _
          .SetConcreteType(handlerType1)
          .SetInterfaceType(interfaceType1)
          .SetBus(bus1)
          .SetTopic(topic1)
          .SetMessageType(messageType1)
          .SetResultType(resultType1)
          .AddOutputBindingDescriptor(_ => _
            .SetMetadataType(metadataType1)
            .SetMetadataObject(metadataObject1)));
      first = builder.Descriptor;

      builder = new(MockBehavior.Strict);
      builder
        .AddHandlerDescriptor(_ => _
          .SetConcreteType(handlerType2)
          .SetInterfaceType(interfaceType2)
          .SetBus(bus2)
          .SetTopic(topic2)
          .SetMessageType(messageType2)
          .SetResultType(resultType2)
          .AddOutputBindingDescriptor(_ => _
            .SetMetadataType(metadataType2)
            .SetMetadataObject(metadataObject2)));
      second = builder.Descriptor;

      builder = new(MockBehavior.Strict);
      builder
        .AddHandlerDescriptor(_ => _
          .SetConcreteType(handlerType1)
          .SetInterfaceType(interfaceType1)
          .SetBus(bus1)
          .SetTopic(topic1)
          .SetMessageType(messageType1)
          .SetResultType(resultType1)
          .AddOutputBindingDescriptor(_ => _
            .SetMetadataType(metadataType1)
            .SetMetadataObject(metadataObject1)))
        .AddHandlerDescriptor(_ => _
          .SetConcreteType(handlerType2)
          .SetInterfaceType(interfaceType2)
          .SetBus(bus2)
          .SetTopic(topic2)
          .SetMessageType(messageType2)
          .SetResultType(resultType2)
          .AddOutputBindingDescriptor(_ => _
            .SetMetadataType(metadataType2)
            .SetMetadataObject(metadataObject2)));
      expected = builder.Descriptor;

      yield return new object?[] { first, second, expected };

      // Merge of the same handler with different output bindings
      builder = new(MockBehavior.Strict);
      builder
        .AddHandlerDescriptor(_ => _
          .SetConcreteType(handlerType1)
          .SetInterfaceType(interfaceType1)
          .SetBus(bus1)
          .SetTopic(topic1)
          .SetMessageType(messageType1)
          .SetResultType(resultType1)
          .AddOutputBindingDescriptor(_ => _
            .SetMetadataType(metadataType1)
            .SetMetadataObject(metadataObject1)));
      first = builder.Descriptor;

      builder = new(MockBehavior.Strict);
      builder
        .AddHandlerDescriptor(_ => _
          .SetConcreteType(handlerType1)
          .SetInterfaceType(interfaceType1)
          .SetBus(bus1)
          .SetTopic(topic1)
          .SetMessageType(messageType1)
          .SetResultType(resultType1)
          .AddOutputBindingDescriptor(_ => _
            .SetMetadataType(metadataType2)
            .SetMetadataObject(metadataObject2)));
      second = builder.Descriptor;

      builder = new(MockBehavior.Strict);
      builder
        .AddHandlerDescriptor(_ => _
          .SetConcreteType(handlerType1)
          .SetInterfaceType(interfaceType1)
          .SetBus(bus1)
          .SetTopic(topic1)
          .SetMessageType(messageType1)
          .SetResultType(resultType1)
          .AddOutputBindingDescriptor(_ => _
            .SetMetadataType(metadataType1)
            .SetMetadataObject(metadataObject1))
          .AddOutputBindingDescriptor(_ => _
            .SetMetadataType(metadataType2)
            .SetMetadataObject(metadataObject2)));
      expected = builder.Descriptor;

      yield return new object?[] { first, second, expected };

      // Merge of the same handler with output binding in common
      builder = new(MockBehavior.Strict);
      builder
        .AddHandlerDescriptor(_ => _
          .SetConcreteType(handlerType1)
          .SetInterfaceType(interfaceType1)
          .SetBus(bus1)
          .SetTopic(topic1)
          .SetMessageType(messageType1)
          .SetResultType(resultType1)
          .AddOutputBindingDescriptor(_ => _
            .SetMetadataType(metadataType1)
            .SetMetadataObject(metadataObject1))
          .AddOutputBindingDescriptor(_ => _
            .SetMetadataType(metadataType3)
            .SetMetadataObject(metadataObject3)))
        .AddHandlerDescriptor(_ => _
          .SetConcreteType(handlerType2)
          .SetInterfaceType(interfaceType2)
          .SetBus(bus2)
          .SetTopic(topic2)
          .SetMessageType(messageType2)
          .SetResultType(resultType2));
      first = builder.Descriptor;

      builder = new(MockBehavior.Strict);
      builder
        .AddHandlerDescriptor(_ => _
          .SetConcreteType(handlerType1)
          .SetInterfaceType(interfaceType1)
          .SetBus(bus1)
          .SetTopic(topic1)
          .SetMessageType(messageType1)
          .SetResultType(resultType1)
          .AddOutputBindingDescriptor(_ => _
            .SetMetadataType(metadataType2)
            .SetMetadataObject(metadataObject2))
          .AddOutputBindingDescriptor(_ => _
            .SetMetadataType(metadataType3)
            .SetMetadataObject(metadataObject3)))
        .AddHandlerDescriptor(_ => _
          .SetConcreteType(handlerType3)
          .SetInterfaceType(interfaceType3)
          .SetBus(bus3)
          .SetTopic(topic3)
          .SetMessageType(messageType3)
          .SetResultType(resultType3));
      second = builder.Descriptor;

      builder = new(MockBehavior.Strict);
      builder
        .AddHandlerDescriptor(_ => _
          .SetConcreteType(handlerType1)
          .SetInterfaceType(interfaceType1)
          .SetBus(bus1)
          .SetTopic(topic1)
          .SetMessageType(messageType1)
          .SetResultType(resultType1)
          .AddOutputBindingDescriptor(_ => _
            .SetMetadataType(metadataType1)
            .SetMetadataObject(metadataObject1))
          .AddOutputBindingDescriptor(_ => _
            .SetMetadataType(metadataType3)
            .SetMetadataObject(metadataObject3))
          .AddOutputBindingDescriptor(_ => _
            .SetMetadataType(metadataType2)
            .SetMetadataObject(metadataObject2)))
        .AddHandlerDescriptor(_ => _
          .SetConcreteType(handlerType2)
          .SetInterfaceType(interfaceType2)
          .SetBus(bus2)
          .SetTopic(topic2)
          .SetMessageType(messageType2)
          .SetResultType(resultType2))
        .AddHandlerDescriptor(_ => _
          .SetConcreteType(handlerType3)
          .SetInterfaceType(interfaceType3)
          .SetBus(bus3)
          .SetTopic(topic3)
          .SetMessageType(messageType3)
          .SetResultType(resultType3));
      expected = builder.Descriptor;

      yield return new object?[] { first, second, expected };
    }

    private class MockType : TypeDelegator
    {
      private readonly int _id;

      public MockType(int id) : base(typeof(MockType)) => _id = id;

      public override bool Equals(Type? o) => o is MockType mockType && _id == mockType._id;
    }
  }
}
