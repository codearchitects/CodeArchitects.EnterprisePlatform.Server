using CodeArchitects.Platform.Messaging.Bindings;

namespace CodeArchitects.Platform.Messaging.Dapr.Bindings;

public interface IStateStoreOutputMetadata : ITypedOutputMetadata
{
  string StoreName { get; }
  string Key { get; }
}
