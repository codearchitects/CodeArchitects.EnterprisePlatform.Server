using CodeArchitects.Platform.Actors.Metadata;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class ImplementationDescriptor : IImplementationDescriptor
{
  private readonly List<IMethodDescriptor> _methods;

  public ImplementationDescriptor(Type type, IConstructorDescriptor constructor)
  {
    Type = type;
    Constructor = constructor;
    _methods = new();
  }

  public Type Type { get; }

  public IConstructorDescriptor Constructor { get; }

  public IReadOnlyList<IMethodDescriptor> Methods => _methods;

  public void AddMethod(IMethodDescriptor method)
  {
    _methods.Add(method);
  }


  public static ImplementationDescriptor Create(IActorMetadata actorMetadata, IImplementationMetadata implementationMetadata, Type interfaceType)
  {
    ConstructorDescriptor constructor = ConstructorDescriptor.Create(actorMetadata, implementationMetadata.Constructor);
    ImplementationDescriptor implementation = new ImplementationDescriptor(implementationMetadata.ImplementationType, constructor);

    foreach (MethodDescriptor method in MethodDescriptor.CreateMany(actorMetadata.ActorType, implementationMetadata, interfaceType))
    {
      implementation.AddMethod(method);
    }

    return implementation;
  }
}
