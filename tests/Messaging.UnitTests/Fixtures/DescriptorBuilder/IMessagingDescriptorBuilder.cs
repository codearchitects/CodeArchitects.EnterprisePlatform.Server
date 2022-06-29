using CodeArchitects.Platform.Messaging.Descriptors;

namespace CodeArchitects.Platform.Messaging.Fixtures.DescriptorBuilder;

internal interface IMessagingDescriptorBuilder
{
  IMessagingDescriptorBuilder AddHandlerDescriptor(Func<IHandlerDescriptorBuilder, IHandlerDescriptorBuilder> build);

  IMessagingDescriptorBuilder SetDiagnostics(IReadOnlyCollection<HandlerDiagnostics> diagnostics);
}
