using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class NonGenericContextDependencyDescriptor : ContextDependencyDescriptor
{
  public NonGenericContextDependencyDescriptor(ParameterInfo parameter, Type actorType)
    : base(parameter)
  {
    ImplementationType = actorType;
  }

  public override Type ImplementationType { get; }
}
