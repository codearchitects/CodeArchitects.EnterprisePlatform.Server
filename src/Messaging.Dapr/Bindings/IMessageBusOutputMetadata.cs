using CodeArchitects.Platform.Messaging.Bindings;

namespace CodeArchitects.Platform.Messaging.Dapr.Bindings;

public interface IMessageBusOutputMetadata : IOutputMetadata
{
  string BusName { get; }
  string? Topic { get; }
}
