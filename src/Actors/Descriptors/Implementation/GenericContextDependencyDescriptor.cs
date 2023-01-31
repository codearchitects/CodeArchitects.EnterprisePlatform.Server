using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class GenericContextDependencyDescriptor : ContextDependencyDescriptor
{
  public GenericContextDependencyDescriptor(ParameterInfo parameter)
    : base(parameter)
  {
  }

  public override Type ImplementationType => Type.GetGenericArguments()[0];
}
