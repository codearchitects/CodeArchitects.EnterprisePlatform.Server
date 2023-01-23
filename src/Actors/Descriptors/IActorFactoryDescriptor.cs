using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IActorFactoryDescriptor
{
  Type FactoryType { get; }
  
  MethodInfo? CreateAsyncMethod { get; }
  
  MethodInfo GetMethod { get; }
}
