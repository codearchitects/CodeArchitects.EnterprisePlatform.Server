using CodeArchitects.Platform.Actors.Descriptors;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Dapr.Infrastructure;

internal interface IActorHostEmitResult
{
  Type InterfaceType { get; }
  
  Type ClassType { get; }

  MethodInfo GetHostMethod(IMethodDescriptor method);
}
