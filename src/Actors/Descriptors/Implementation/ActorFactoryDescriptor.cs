using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class ActorFactoryDescriptor : IActorFactoryDescriptor
{
  public ActorFactoryDescriptor(Type factoryType, MethodInfo? createAsyncMethod, MethodInfo getMethod)
  {
    FactoryType = factoryType;
    CreateAsyncMethod = createAsyncMethod;
    GetMethod = getMethod;
  }

  public Type FactoryType { get; }

  public MethodInfo? CreateAsyncMethod { get; }

  public MethodInfo GetMethod { get; }
}
