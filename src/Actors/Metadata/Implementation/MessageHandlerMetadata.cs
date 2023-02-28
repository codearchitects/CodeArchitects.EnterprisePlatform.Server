namespace CodeArchitects.Platform.Actors.Metadata.Implementation;

internal class MessageHandlerMetadata : IMessageHandlerMetadata
{
  public MessageHandlerMetadata(string? bus, string? topic)
  {
    Bus = bus;
    Topic = topic;
  }

  public string? Bus { get; }

  public string? Topic { get; }
}
