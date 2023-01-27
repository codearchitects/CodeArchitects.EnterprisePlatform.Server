using CodeArchitects.Platform.Actors.Metadata.Implementation;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Reflection;

internal class ReflectionImplementationMetadata : ImplementationMetadata
{
  public ReflectionImplementationMetadata(Type implementationType)
  {
    ImplementationType = implementationType;
  }

  public override bool IsDefault => ImplementationType.GetCustomAttribute<ActorImplementationAttribute>() is { } implementationAttribute && implementationAttribute.IsDefault;

  public override Type ImplementationType { get; }

  public override ConstructorInfo? Constructor
  {
    get
    {
      try
      {
        return ImplementationType
          .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
          .Where(ctor => ctor.IsDefined(typeof(ActorConstructorAttribute)))
          .SingleOrDefault();
      }
      catch (InvalidOperationException)
      {
        throw InvalidActorException.AmbiguousActorConstructor(ImplementationType);
      }
    }
  }

  public override bool HasStateFields => ImplementationType
    .GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
    .Any(member => member.IsDefined(typeof(StateAttribute)));

  public static ReflectionImplementationMetadata Create(Type implementationType)
  {
    ReflectionImplementationMetadata metadata = new(implementationType);

    foreach (MethodInfo implementationMethod in implementationType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
    {
      metadata.AddMethodMetadata(new ReflectionMethodMetadata(implementationMethod));
    }

    return metadata;
  }
}
