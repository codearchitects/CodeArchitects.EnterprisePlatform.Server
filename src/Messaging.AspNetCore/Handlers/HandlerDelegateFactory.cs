using CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.Messaging.AspNetCore.Bindings;
using CodeArchitects.Platform.Messaging.Descriptors;
using OneOf;

namespace CodeArchitects.Platform.Messaging.AspNetCore.Handlers;

/// <summary>
/// Implementation of <see cref="IHandlerDelegateFactory"/>.
/// </summary>
internal class HandlerDelegateFactory : IHandlerDelegateFactory
{
  private static readonly HashSet<Type> s_supportedOneOfTypes = new HashSet<Type>
  {
    typeof(OneOf<,>),
    typeof(OneOf<,,>),
    typeof(OneOf<,,,>),
    typeof(OneOf<,,,,>),
    typeof(OneOf<,,,,,>),
    typeof(OneOf<,,,,,,>),
    typeof(OneOf<,,,,,,,>),
    typeof(OneOf<,,,,,,,,>)
  };

  private readonly IServiceProvider _services;
  private readonly IOutputActionFactory _outputActionFactory;

  /// <summary>
  /// Creates a new <see cref="HandlerDelegateFactory"/> instance.
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
    if (!descriptor.HasResult)
      return CreateDelegateNoResult(descriptor);

    if (!descriptor.HasUnionResult)
      return CreateDelegateWithResult(descriptor);

    return CreateDelegateOneOf(descriptor);
  }

  private HandlerDelegate CreateDelegateNoResult(IHandlerDescriptor descriptor)
  {
    IReadOnlyCollection<OutputAction> outputActions = CreateOutputActions(descriptor.OutputBindingDescriptors)
      .Where(action => !action.IsTypeFiltered)
      .ToList();
    Type handlerDelegateType = typeof(HandlerDelegate<,>).MakeGenericType(descriptor.ConcreteType, descriptor.MessageType);

    return (HandlerDelegate)Activator.CreateInstance(handlerDelegateType, new object?[] { outputActions })!;
  }

  private HandlerDelegate CreateDelegateWithResult(IHandlerDescriptor descriptor) 
  {
    Type resultType = descriptor.ResultType;
    IReadOnlyCollection<OutputAction> outputActions = CreateOutputActions(descriptor.OutputBindingDescriptors)
      .Where(action => !action.IsTypeFiltered || action.CanExecute(resultType))
      .ToList();
    Type handlerDelegateType = typeof(HandlerDelegate<,,>).MakeGenericType(descriptor.ConcreteType, descriptor.MessageType, resultType);

    return (HandlerDelegate)Activator.CreateInstance(handlerDelegateType, new object?[] { outputActions })!;
  }

  private HandlerDelegate CreateDelegateOneOf(IHandlerDescriptor descriptor)
  {
    Type resultType = descriptor.ResultType;
    if (!resultType.IsGenericType || !s_supportedOneOfTypes.Contains(resultType.GetGenericTypeDefinition()))
      throw new NotSupportedException($"Type '{resultType}' is not a supported union type.");
    
    IReadOnlyList<Type> resultTypes = descriptor.ResultTypes;
    int arity = resultTypes.Count;

    IEnumerable<OutputAction> outputActions = CreateOutputActions(descriptor.OutputBindingDescriptors);
    IEnumerable<OutputAction>[] outputActionsArray = new IEnumerable<OutputAction>[arity];

    for (int i = 0; i < arity; i++)
    {
      outputActionsArray[i] = outputActions
        .Where(action => !action.IsTypeFiltered || action.CanExecute(resultTypes[i]))
        .ToList();
    }

    Type handlerDelegateType = arity switch
    {
      2 => typeof(HandlerDelegate<,,,>),
      3 => typeof(HandlerDelegate<,,,,>),
      4 => typeof(HandlerDelegate<,,,,,>),
      5 => typeof(HandlerDelegate<,,,,,,>),
      6 => typeof(HandlerDelegate<,,,,,,,>),
      7 => typeof(HandlerDelegate<,,,,,,,,>),
      8 => typeof(HandlerDelegate<,,,,,,,,,>),
      9 => typeof(HandlerDelegate<,,,,,,,,,,>),
      _ => throw Errors.Unreacheable
    };

    Type[] handlerDelegateTypeArguments = new Type[2 + arity];
    handlerDelegateTypeArguments[0] = descriptor.ConcreteType;
    handlerDelegateTypeArguments[1] = descriptor.MessageType;
    for (int i = 0; i < resultTypes.Count; i++)
    {
      handlerDelegateTypeArguments[i + 2] = resultTypes[i];
    }
    handlerDelegateType = handlerDelegateType.MakeGenericType(handlerDelegateTypeArguments);

    return (HandlerDelegate)Activator.CreateInstance(handlerDelegateType, new object?[] { outputActionsArray })!;
  }

  private IEnumerable<OutputAction> CreateOutputActions(IReadOnlyCollection<IOutputBindingDescriptor> outputBindingDescriptors)
  {
    if (outputBindingDescriptors.Count == 0)
      return Array.Empty<OutputAction>();

    return outputBindingDescriptors
      .Select(descriptor => _outputActionFactory.CreateOutputAction(descriptor.MetadataType, descriptor.MetadataObject, _services));
  }
}
