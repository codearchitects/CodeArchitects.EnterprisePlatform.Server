using CodeArchitects.Platform.Messaging.Fixtures;
using CodeArchitects.Platform.Messaging.Fixtures.DescriptorBuilder;
using Moq;
using System.Reflection;
using Xunit.Sdk;

namespace CodeArchitects.Platform.Messaging.Descriptors.Reflection;

public static class HandlerDescriptorFixture
{
  internal class StandardMessageHandlerDataAttribute : DataAttribute
  {
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      Type[] concreteTypes;
      HandlerDiagnostics[] diagnostics;
      MessagingDescriptorBuilder builder;

      // With defaultBus != null and defaultTopic != null
      string defaultBus = "defaultBus";
      string defaultTopic = "defaultTopic";
      concreteTypes = new[] { typeof(StandardMessageHandler), typeof(NoBusAndTopicMessageHandler) };
      diagnostics = Array.Empty<HandlerDiagnostics>();
      builder = new(MockBehavior.Strict);
      AddStandardMessageHandler1(builder);
      AddStandardMessageHandler2(builder);
      AddNoBusAndTopicMessageHandler1(builder, defaultBus, defaultTopic);
      builder.SetDiagnostics(diagnostics);

      yield return new object?[] { concreteTypes, defaultBus, defaultTopic, builder.Descriptor };

      // With defaultBus == null and defaultTopic == null
      concreteTypes = new[] { typeof(StandardMessageHandler), typeof(NoBusAndTopicMessageHandler) };
      diagnostics = new[]
      {
        HandlerDiagnostics.NullBusOnHandler(typeof(NoBusAndTopicMessageHandler), NoBusAndTopicMessageHandlerInfo.HandlerMethod1),
        HandlerDiagnostics.NullTopicOnHandler(typeof(NoBusAndTopicMessageHandler), NoBusAndTopicMessageHandlerInfo.HandlerMethod1)
      };
      builder = new(MockBehavior.Strict);
      AddStandardMessageHandler1(builder);
      AddStandardMessageHandler2(builder);
      builder.SetDiagnostics(diagnostics);

      yield return new object?[] { concreteTypes, null, null, builder.Descriptor };

      // For Id = 1 diagnostic
      concreteTypes = new[] { typeof(MultipleMessageHandlerAttributesHandler) };
      diagnostics = new[]
      {
        HandlerDiagnostics.MultipleMessageHandlerAttributeOnClass(typeof(MultipleMessageHandlerAttributesHandler))
      };
      builder = new(MockBehavior.Strict);
      builder.SetDiagnostics(diagnostics);

      yield return new object?[] { concreteTypes, null, null, builder.Descriptor };
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
          .SetResultType(typeof(void)))
        .AddMessageDescriptor(_ => _
          .SetName(Message1.MessageName)
          .SetType(typeof(Message1)));
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

    private static void AddNoBusAndTopicMessageHandler1(IMessagingDescriptorBuilder builder, string defaultBus, string defaultTopic)
    {
      builder
        .AddHandlerDescriptor(_ => _
          .SetBus(defaultBus)
          .SetTopic(defaultTopic)
          .SetInterfaceType(typeof(IMessageHandler<Message3>))
          .SetConcreteType(typeof(NoBusAndTopicMessageHandler))
          .SetMessageType(typeof(Message3))
          .SetResultType(typeof(void)))
        .AddMessageDescriptor(_ => _
          .SetName(typeof(Message3).Name)
          .SetType(typeof(Message3)));
    }
  }
}
