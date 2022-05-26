using CodeArchitects.Platform.Messaging.Fixtures;
using CodeArchitects.Platform.Messaging.Fixtures.DescriptorBuilder;
using Moq;
using System.Reflection;
using Xunit.Sdk;

namespace CodeArchitects.Platform.Messaging.Descriptors.Concrete;

public static class HandlerDescriptorFixture
{
  internal class StandardHandlerDataAttribute : DataAttribute
  {
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
      HandlerDescriptorBuilder builder = new(MockBehavior.Strict);

      builder
        .SetConcreteType(typeof(StandardMessageHandler))
        .AddIdentityDescriptor(_ => _
          .SetInterfaceType(typeof(IMessageHandler<Message1>))
          .SetMessageType(typeof(Message1))
          .SetResultType(null)
          .SetBus(StandardMessageHandlerInfo.Identity1Bus)
          .SetTopic(StandardMessageHandlerInfo.Identity1Topic))
        .AddIdentityDescriptor(_ => _
          .SetInterfaceType(typeof(IMessageHandler<Message2, Message1>))
          .SetMessageType(typeof(Message2))
          .SetResultType(typeof(Message1))
          .SetBus(StandardMessageHandlerInfo.ClassBus)
          .SetTopic(StandardMessageHandlerInfo.ClassTopic)
          .AddOutputBindingDescriptor(_ => _
            .SetMetadataType(typeof(IFakeOutputMetadata1))
            .SetMetadataObject(StandardMessageHandlerInfo.Identity2MetadataObject)));

      yield return new[] { builder.Descriptor };
    }
  }
}
