using CodeArchitects.Platform.Messaging.Bindings;

namespace CodeArchitects.Platform.Messaging.Dapr.Bindings;

public interface IMessageBusOutputMetadata : ITypedOutputMetadata
{
  string? Bus { get; }

  string? Topic { get; }
}
