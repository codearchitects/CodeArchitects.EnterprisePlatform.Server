using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IMessageHandlerDescriptor
{
  Type InterfaceType { get; }

  Type MessageType { get; }

  Type ResultType { get; }

  MethodInfo InterfaceMethod { get; }

  IMethodDescriptor Activity { get; }

  IReadOnlyCollection<IMessageHandlerMetadata> HandlerMetadataCollection { get; }
}
