namespace CodeArchitects.Platform.Actors.Descriptors.Builder;

public interface IMessageHandlerMetadataBuilder
{
  IMessageHandlerMetadataBuilder WithBus(string bus);
  IMessageHandlerMetadataBuilder WithTopic(string topic);
}
