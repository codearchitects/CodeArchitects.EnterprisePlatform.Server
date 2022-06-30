using CodeArchitects.Platform.Messaging.AspNetCore.Bindings;
using CodeArchitects.Platform.Messaging.Descriptors;

namespace CodeArchitects.Platform.Messaging.AspNetCore;

/// <summary>
/// Implementation of <see cref="IHandlerDelegateFactory"/>.
/// </summary>
internal class HandlerDelegateFactory : IHandlerDelegateFactory
{
  private readonly IServiceProvider _services;
  private readonly IOutputActionFactory _outputActionFactory;

  /// <summary>
  /// Creates a new <see cref="HandlerDelegateFactory"/>.
  /// </summary>
  /// <param name="services">The service provider.</param>
  /// <param name="outputActionFactory">A factory of output actions.</param>
  public HandlerDelegateFactory(IServiceProvider services, IOutputActionFactory outputActionFactory)
  {
    _services = services;
    _outputActionFactory = outputActionFactory;
  }

  public HandlerDelegate CreateHandlerDelegate(IHandlerDescriptor descriptor)
  {
    IReadOnlyList<OutputAction> outputActions = descriptor.OutputBindingDescriptors
      .Select(descriptor => _outputActionFactory.CreateOutputAction(descriptor.MetadataType, descriptor.MetadataObject, _services))
      .ToList();
    Type resultType = descriptor.ResultType;

    Type handlerDelegateType = resultType == typeof(void)
      ? typeof(HandlerDelegate<,>).MakeGenericType(descriptor.MessageType, descriptor.ConcreteType)
      : typeof(HandlerDelegate<,,>).MakeGenericType(descriptor.MessageType, resultType, descriptor.ConcreteType);

    return (HandlerDelegate)Activator.CreateInstance(handlerDelegateType, new[] { outputActions })!;
  }
}
