using CodeArchitects.Platform.Actors.Descriptors;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Dapr.Infrastructure;

internal class ActorHostEmitResult : IActorHostEmitResult
{
  private readonly IReadOnlyDictionary<IMethodDescriptor, MethodInfo> _methods;

  public ActorHostEmitResult(Type interfaceType, Type classType, Type? handlerInterfaceType, IReadOnlyDictionary<IMethodDescriptor, MethodInfo> methods)
  {
    InterfaceType = interfaceType;
    ClassType = classType;
    HandlerInterfaceType = handlerInterfaceType;
    _methods = methods;
  }

  public Type InterfaceType { get; }

  public Type ClassType { get; }
  public Type? HandlerInterfaceType { get; }

  public MethodInfo GetHostMethod(IMethodDescriptor method)
  {
    return _methods[method];
  }
}
