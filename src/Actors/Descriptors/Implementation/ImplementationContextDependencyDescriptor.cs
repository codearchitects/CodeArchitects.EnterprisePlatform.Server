using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class ImplementationContextDependencyDescriptor : ContextDependencyDescriptor
{
  public ImplementationContextDependencyDescriptor(ParameterInfo parameter)
    : base(parameter)
  {
  }

  public override Type ImplementationType => Type.GetGenericArguments()[1];
}
