namespace CodeArchitects.Platform.Actors.Metadata;

internal interface IMessageHandlerMetadata
{
  string? Bus { get; }
  string? Topic { get; }
}
