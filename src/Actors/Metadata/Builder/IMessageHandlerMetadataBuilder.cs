namespace CodeArchitects.Platform.Actors.Metadata.Builder;

public interface IMessageHandlerMetadataBuilder
{
  IMessageHandlerMetadataBuilder WithBus(string bus);
  IMessageHandlerMetadataBuilder WithTopic(string topic);
}
