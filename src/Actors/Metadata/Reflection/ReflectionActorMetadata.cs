using CodeArchitects.Platform.Actors.Metadata.Implementation;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Reflection;

internal class ReflectionActorMetadata : ActorMetadata
{
  private readonly Type _actorType;
  private readonly IActorAttribute _actorAttribute;

  public ReflectionActorMetadata(Type actorType, IActorAttribute actorAttribute, Type? factoryType)
  {
    _actorType = actorType;
    _actorAttribute = actorAttribute;
    FactoryType = factoryType;
  }

  public override Type? InterfaceType => _actorAttribute.InterfaceType;

  public override bool IsExplicitVirtual => ActorType.IsDefined(typeof(VirtualAttribute));

  public override IImplementationMetadata BaseImplementation => ReflectionImplementationMetadata.Create(_actorType);

  private void AddImplementation(Type implementationType)
  {
    AddImplementation(ReflectionImplementationMetadata.Create(implementationType));
  }

  private void AddStateField(FieldInfo field)
  {
    AddStateField(new FieldReflectionStateFieldMetadata(field));
  }

  private void AddStateProperty(PropertyInfo property)
  {
    if (!property.TryGetBackingFieldByConvention(BackingFieldNameConvention.AutoGen, out FieldInfo? backingField))
      throw InvalidActorException.InvalidStateProperty(_actorType, property);

    AddStateField(new PropertyReflectionStateFieldMetadata(backingField, property));
  }

  public static ReflectionActorMetadata Create(ReflectionMetadataSource source)
  {
    ReflectionActorMetadata metadata = new(source.ActorType, source.ActorAttribute, source.FactoryType);

    foreach (FieldInfo field in source.ActorType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
    {
      if (!field.IsDefined(typeof(StateAttribute)))
        continue;

      metadata.AddStateField(field);
    }

    foreach (PropertyInfo property in source.ActorType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
    {
      if (!property.IsDefined(typeof(StateAttribute)))
        continue;

      metadata.AddStateProperty(property);
    }

    foreach (Type implementationType in source.ImplementationTypes)
    {
      metadata.AddImplementation(implementationType);
    }

    return metadata;
  }
}
