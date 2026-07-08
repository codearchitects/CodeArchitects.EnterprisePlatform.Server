using CodeArchitects.Platform.Messaging.Bindings;

namespace CodeArchitects.Platform.Messaging.Dapr.Bindings;

public interface IStateStoreOutputMetadata : ITypedOutputMetadata
{
  string Store { get; }

  string Key { get; }
}
