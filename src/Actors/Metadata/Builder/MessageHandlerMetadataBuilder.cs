using CodeArchitects.Platform.Actors.Metadata.Implementation;

namespace CodeArchitects.Platform.Actors.Metadata.Builder;

internal class MessageHandlerMetadataBuilder : IMessageHandlerMetadataBuilder
{
  private string? _bus;
  private string? _topic;

  public IMessageHandlerMetadataBuilder WithBus(string bus)
  {
    _bus = bus;
    return this;
  }

  public IMessageHandlerMetadataBuilder WithTopic(string topic)
  {
    _topic = topic;
    return this;
  }

  public MessageHandlerMetadata Build() => new(_bus, _topic);
}
