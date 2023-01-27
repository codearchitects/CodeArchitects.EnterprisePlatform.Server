using CodeArchitects.Platform.Actors.Metadata;
using System.Reflection;

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
    Type actorType = actorMetadata.ActorType;
    Type implementationType = implementationMetadata.ImplementationType;

    if (actorType != implementationType && implementationMetadata.HasStateFields)
      throw InvalidActorException.StateMustBeDefinedInBaseActor(actorType, implementationType);

    ConstructorDescriptor constructor = ConstructorDescriptor.Create(actorMetadata, GetConstructor(implementationMetadata));
    ImplementationDescriptor implementation = new ImplementationDescriptor(implementationType, constructor);

    foreach (MethodDescriptor method in MethodDescriptor.CreateMany(actorMetadata, implementationMetadata, interfaceType))
    {
      implementation.AddMethod(method);
    }

    return implementation;
  }

  private static ConstructorInfo GetConstructor(IImplementationMetadata implementationMetadata)
  {
    if (implementationMetadata.Constructor is { } constructor)
      return constructor;

    try
    {
      return implementationMetadata.ImplementationType
        .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        .Single();
    }
    catch (InvalidOperationException)
    {
      throw InvalidActorException.AmbiguousActorConstructor(implementationMetadata.ImplementationType);
    }
  }
}
