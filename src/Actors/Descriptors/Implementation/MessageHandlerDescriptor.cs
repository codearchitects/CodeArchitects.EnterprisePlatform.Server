using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class MessageHandlerDescriptor : IMessageHandlerDescriptor
{
  public MessageHandlerDescriptor(
    Type interfaceType,
    Type messageType,
    Type resultType,
    IMethodDescriptor activity,
    IReadOnlyCollection<IMessageHandlerMetadata> handlerMetadataCollection)
  {
    InterfaceType = interfaceType;
    MessageType = messageType;
    ResultType = resultType;
    Activity = activity;
    HandlerMetadataCollection = handlerMetadataCollection;
  }
  
  public Type InterfaceType { get; }
  
  public Type MessageType { get; }

  public Type ResultType { get; }

  public IMethodDescriptor Activity { get; }

  public IReadOnlyCollection<IMessageHandlerMetadata> HandlerMetadataCollection { get; }

  public MethodInfo InterfaceMethod => InterfaceType.GetRequiredMethod(
    name: "HandleAsync",
    bindingAttr: BindingFlags.Instance | BindingFlags.Public,
    types: new[] { MessageType, typeof(CancellationToken) });
}
