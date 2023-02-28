using CodeArchitects.Platform.Actors.Metadata;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Dapr.Infrastructure;

internal interface IActorHostEmitResult
{
  Type InterfaceType { get; }
  
  Type ClassType { get; }

  Type? HandlerInterfaceType { get; }

  MethodInfo GetHostMethod(IMethodDescriptor method);
}
