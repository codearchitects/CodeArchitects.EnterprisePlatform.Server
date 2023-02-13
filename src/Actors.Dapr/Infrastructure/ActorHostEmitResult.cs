using CodeArchitects.Platform.Actors.Descriptors;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Dapr.Infrastructure;

internal class ActorHostEmitResult : IActorHostEmitResult
{
  private readonly IReadOnlyDictionary<IMethodDescriptor, MethodInfo> _methods;

  public ActorHostEmitResult(Type interfaceType, Type classType, IReadOnlyDictionary<IMethodDescriptor, MethodInfo> methods)
  {
    InterfaceType = interfaceType;
    ClassType = classType;
    _methods = methods;
  }

  public Type InterfaceType { get; }

  public Type ClassType { get; }

  public MethodInfo GetHostMethod(IMethodDescriptor method)
  {
    return _methods[method];
  }
}
