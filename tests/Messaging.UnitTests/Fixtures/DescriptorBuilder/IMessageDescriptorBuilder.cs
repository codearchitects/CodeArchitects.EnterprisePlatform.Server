namespace CodeArchitects.Platform.Messaging.Fixtures.DescriptorBuilder;

internal interface IMessageDescriptorBuilder
{
  IMessageDescriptorBuilder SetName(string name);

  IMessageDescriptorBuilder SetType(Type type);
}
