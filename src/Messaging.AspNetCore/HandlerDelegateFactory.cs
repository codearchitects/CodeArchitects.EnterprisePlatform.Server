using CodeArchitects.Platform.Messaging.AspNetCore.Bindings;
using CodeArchitects.Platform.Messaging.Descriptors;

namespace CodeArchitects.Platform.Messaging.AspNetCore;

/// <summary>
/// Implementation of <see cref="IHandlerDelegateFactory"/>.
/// </summary>
internal class HandlerDelegateFactory : IHandlerDelegateFactory
{
  private readonly IServiceProvider _services;

  /// <summary>
  /// Creates a new <see cref="HandlerDelegateFactory"/>.
  /// </summary>
  /// <param name="services">The service provider.</param>
  public HandlerDelegateFactory(IServiceProvider services)
  {
    _services = services;
  }

  public HandlerDelegate CreateHandlerDelegate(IHandlerDescriptor identityDescriptor)
  {
    IReadOnlyList<OutputAction> outputActions = identityDescriptor.OutputBindingDescriptors
      .Select(descriptor => OutputAction.Create(descriptor.MetadataType, descriptor.MetadataObject, _services))
      .ToList();
    Type resultType = identityDescriptor.ResultType;

    return resultType == typeof(void)
      ? HandlerDelegate.CreateNoResult(outputActions, identityDescriptor.MessageType, identityDescriptor.ConcreteType)
      : HandlerDelegate.CreateWithResult(outputActions, identityDescriptor.MessageType, resultType, identityDescriptor.ConcreteType);
  }
}
