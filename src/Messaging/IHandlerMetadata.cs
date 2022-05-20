namespace CodeArchitects.Platform.Messaging;

internal interface IHandlerMetadata
{
  string? Bus { get; }
  string? Topic { get; }
}
