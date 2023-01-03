using CodeArchitects.Platform.Messaging.Bindings;

namespace CodeArchitects.Platform.Messaging.Dapr.Bindings;

internal interface IStateStoreOutputMetadata : ITypedOutputMetadata
{
  string Store { get; }
  string Key { get; }
}
