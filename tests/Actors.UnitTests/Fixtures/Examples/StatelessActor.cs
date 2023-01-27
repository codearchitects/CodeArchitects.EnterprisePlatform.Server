using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.FluentMock;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.Metadata.FluentMock;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Fixtures.Examples;

internal interface IStatelessActor
{
}

[Actor]
internal class StatelessActor : IStatelessActor
{
  private readonly IService1 _service1;

  public StatelessActor(IService1 service1)
  {
    _service1 = service1;
  }
}

[ActorFactory(typeof(StatelessActor))]
internal interface IStatelessActorFactory
{
  IStatelessActor Get(string id);
}

internal static class StatelessActorFixture
{
  public static readonly IActorDescriptor Descriptor;
  public static readonly IActorMetadata Metadata;

  private static readonly ConstructorInfo s_constructor;

  static StatelessActorFixture()
  {
    s_constructor = typeof(StatelessActor).GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(IService1) });

    ParameterInfo[] constructorParameters = s_constructor.GetParameters();

    MethodInfo factoryGetMethod = typeof(IStatelessActorFactory).GetRequiredMethod(
      name: nameof(IStatelessActorFactory.Get),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(string) });


    IServiceDependencyDescriptor service1Dependency = ServiceDependencyDescriptorBuilder.Build(_ => _
      .InitDefaults()
      .SetParameter(constructorParameters[0])
      .SetName("service1")
      .SetType(typeof(IService1))
      .SetIndex(0)
      .SetIsOptional(false));

    IImplementationDescriptor implementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetType(typeof(StatelessActor))
      .SetConstructor(_ => _
        .SetConstructor(s_constructor)
        .SetDependencies(service1Dependency)
        .SetContextDependency(null as IContextDependencyDescriptor)
        .SetServiceDependencies(service1Dependency)
        .SetStateDependencies())
      .SetMethods());

    Descriptor = ActorDescriptorBuilder.Build(_ => _
      .SetInterfaceType(typeof(IStatelessActor))
      .SetActorType(typeof(StatelessActor))
      .SetBaseImplementation(implementation)
      .SetDefaultImplementation(implementation)
      .SetImplementations(implementation)
      .SetIsPolymorphic(false)
      .SetId(_ => _
        .SetIdType(typeof(string))
        .SetHasIdSource(false)
        .SetStateDependency(null as IStateDependencyDescriptor)
        .SetStateProperty(null))
      .SetState(_ => _
        .SetStateType(typeof(NoState))
        .SetIsStateless(true)
        .SetIsVirtual(true)
        .SetFields()
        .SetDefaultValues())
      .SetFactory(_ => _
        .SetFactoryType(typeof(IStatelessActorFactory))
        .SetCreateAsyncMethod(null)
        .SetGetMethod(factoryGetMethod)));


    IImplementationMetadata baseImplementationMetadata = ImplementationMetadataBuilder.Build(_ => _
      .SetIsDefault(false)
      .SetImplementationType(typeof(StatelessActor))
      .SetConstructor(null)
      .SetHasStateFields(false));

    Metadata = ActorMetadataBuilder.Build(_ => _
      .SetInterfaceType(typeof(IStatelessActor))
      .SetActorType(typeof(StatelessActor))
      .SetIsExplicitVirtual(false)
      .SetFactoryType(typeof(IStatelessActorFactory))
      .SetStateFields()
      .SetBaseImplementation(baseImplementationMetadata)
      .SetImplementations());
  }

  public static void AssertValidMetadata(IActorMetadata metadata, bool hasConstructor)
  {
    metadata.InterfaceType.Should().BeNull();
    metadata.ActorType.Should().Be<StatelessActor>();
    metadata.IsExplicitVirtual.Should().BeFalse();
    metadata.FactoryType.Should().Be<IStatelessActorFactory>();
    metadata.StateFields.Should().HaveCount(0);

    metadata.BaseImplementation.IsDefault.Should().BeFalse();
    metadata.BaseImplementation.ImplementationType.Should().Be<StatelessActor>();
    metadata.BaseImplementation.HasStateFields.Should().BeFalse();
    if (hasConstructor)
    {
      metadata.BaseImplementation.Constructor.Should().BeSameAs(s_constructor);
    }
    else
    {
      metadata.BaseImplementation.Constructor.Should().BeNull();
    }

    metadata.Implementations.Should().BeEmpty();
  }
}
