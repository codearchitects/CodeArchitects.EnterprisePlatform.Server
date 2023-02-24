namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IMessageHandlerMetadata
{
  string? Bus { get; }
  string? Topic { get; }
}
