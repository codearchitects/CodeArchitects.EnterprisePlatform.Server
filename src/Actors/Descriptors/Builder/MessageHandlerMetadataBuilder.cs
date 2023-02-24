using CodeArchitects.Platform.Actors.Descriptors.Implementation;

namespace CodeArchitects.Platform.Actors.Descriptors.Builder;

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
