using CodeArchitects.Platform.Actors.Descriptors.Factory;
using CodeArchitects.Platform.Actors.Descriptors.Implementation;
using CodeArchitects.Platform.Actors.Messaging;
using CodeArchitects.Platform.Actors.Scheduling;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Reflection;

internal class ReflectionActorDescriptorFactory<TActor> : ActorDescriptorFactory<TActor>
  where TActor : class
{
  private readonly IReflectionMetadataContext _context;
  private readonly IActorAttribute _actorAttribute;
  private IReadOnlyCollection<StateComponentMetadata<TActor>>? _stateComponents;
  private IReadOnlyCollection<ImplementationDescriptorFactory<TActor>>? _implementationFactories;

  public ReflectionActorDescriptorFactory(IStateTypeBuilder stateTypeBuilder, IActivityTypeBuilder activityTypeBuilder, IReflectionMetadataContext context, IActorAttribute actorAttribute)
    : base(stateTypeBuilder, activityTypeBuilder)
  {
    _context = context;
    _actorAttribute = actorAttribute;
    BaseImplementationFactory = new ReflectionImplementationDescriptorFactory<TActor>(0, this, ActorType);
  }

  protected override Type? InterfaceType => _actorAttribute.InterfaceType;

  protected override Type? FactoryType => _context.GetFactoryType(ActorType);

  protected override bool IsExplicitVirtual => ActorType.IsDefined(typeof(VirtualAttribute));

  protected override IReadOnlyCollection<StateComponentMetadata<TActor>> StateComponents => _stateComponents ??= CreateStateComponents();

  protected override IEnumerable<MemberMetadata> ActorIdMembers
  {
    get
    {
      MemberInfo[] members = ActorType
        .GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      foreach (MemberInfo member in members)
      {
        if (!member.IsDefined(typeof(ActorIdAttribute)) || member.IsDefined(typeof(StateAttribute)))
          continue;

        switch (member)
        {
          case FieldInfo field:
            yield return new MemberMetadata(field, field.FieldType);
            break;
          case PropertyInfo property:
            yield return new MemberMetadata(property, property.PropertyType);
            break;
        }
      }
    }
  }

  protected override ImplementationDescriptorFactory<TActor> BaseImplementationFactory { get; }

  protected override IReadOnlyCollection<ImplementationDescriptorFactory<TActor>> ImplementationFactories => _implementationFactories ??= CreateImplementationFactories();

  private IReadOnlyCollection<StateComponentMetadata<TActor>> CreateStateComponents()
  {
    List<StateComponentMetadata<TActor>> stateComponents = new();

    MemberInfo[] members = ActorType
      .GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    foreach (MemberInfo member in members)
    {
      if (!member.IsDefined(typeof(StateAttribute)))
        continue;

      int id = stateComponents.Count;
      switch (member)
      {
        case FieldInfo field:
          stateComponents.Add(new ReflectionStateComponentMetadata<TActor>(id, field, field.FieldType));
          break;
        case PropertyInfo property:
          stateComponents.Add(new ReflectionStateComponentMetadata<TActor>(id, property, property.PropertyType));
          break;
      }
    }

    return stateComponents;
  }
  
  private IReadOnlyCollection<ImplementationDescriptorFactory<TActor>> CreateImplementationFactories()
  {
    List<ImplementationDescriptorFactory<TActor>> implementationFactories = new();

    foreach (Type implementationType in _context.GetImplementationTypes(ActorType))
    {
      int id = implementationFactories.Count + 1;
      implementationFactories.Add(new ReflectionImplementationDescriptorFactory<TActor>(id, this, implementationType));
    }

    return implementationFactories;
  }

  protected override IReadOnlyCollection<IMessageHandlerMetadata> GetMessageHandlerMetadataCollection(IMethodDescriptor activity)
  {
    List<IMessageHandlerMetadata> result = new();

    foreach (Attribute attribute in activity.ImplementationMethod.GetCustomAttributes())
    {
      if (!MessagingMetadata.Metadata.IsMessageHandlerAttribute(attribute))
        continue;

      string? bus = (string?)MessagingMetadata.Metadata.AttributeBusProperty.GetValue(attribute);
      string? topic = (string?)MessagingMetadata.Metadata.AttributeTopicProperty.GetValue(attribute);

      result.Add(new MessageHandlerMetadata(bus, topic));
    }

    return result;
  }
}
