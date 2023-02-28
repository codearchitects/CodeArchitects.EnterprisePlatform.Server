using CodeArchitects.Platform.Actors.Metadata.Factory;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Reflection;

internal class ReflectionImplementationDescriptorFactory<TActor> : ImplementationDescriptorFactory<TActor>
  where TActor : class
{
  public ReflectionImplementationDescriptorFactory(int id, ReflectionActorDescriptorFactory<TActor> factory, Type implementationType)
    : base(id, factory)
  {
    ImplementationType = implementationType;
  }

  public override bool IsDefault =>
    ImplementationType.GetCustomAttribute<ActorImplementationAttribute>(inherit: false)?.IsDefault ??
    ImplementationType.GetCustomAttribute<ActorImplementationAttribute<TActor>>(inherit: false)!.IsDefault;

  public override Type ImplementationType { get; }

  protected override bool HasStateFields => ImplementationType
    .GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
    .Where(member => member.DeclaringType != typeof(TActor))
    .Any(member => member.IsDefined(typeof(StateAttribute)));

  protected override ConstructorInfo? Constructor
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
}
