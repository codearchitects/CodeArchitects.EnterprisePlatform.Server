using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.FluentMock;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.Metadata.FluentMock;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Fixtures.Examples;

internal interface IStatelessActor
{
}

internal class StatelessActor : IStatelessActor
{
  private readonly IService1 _service1;

  public StatelessActor(IService1 service1)
  {
    _service1 = service1;
  }
}

internal interface IStatelessActorFactory
{
  IStatelessActor Get(string id);
}

internal static class StatelessActorFixture
{
  public static readonly IActorDescriptor Descriptor;
  public static readonly IActorMetadata Metadata;

  static StatelessActorFixture()
  {
    ConstructorInfo constructorInfo = typeof(StatelessActor).GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(IService1) });

    ParameterInfo[] constructorParameters = constructorInfo.GetParameters();

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
      .SetCategoryIndex(0)
      .SetIsOptional(false));

    IConstructorDescriptor constructor = ConstructorDescriptorBuilder.Build(_ => _
      .SetConstructor(constructorInfo)
      .SetDependencies(service1Dependency)
      .SetContextDependency(null as IContextDependencyDescriptor)
      .SetServiceDependencies(service1Dependency)
      .SetStateDependencies());

    IImplementationDescriptor implementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetType(typeof(StatelessActor))
      .SetConstructor(constructor)
      .SetMethods());

    Descriptor = ActorDescriptorBuilder.Build(_ => _
      .SetInterfaceType(typeof(IStatelessActor))
      .SetActorType(typeof(StatelessActor))
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
        .SetGetMethod(factoryGetMethod))
      .SetConstructor(constructor));


    Metadata = ActorMetadataBuilder.Build(_ => _
      .SetInterfaceType(typeof(IStatelessActor))
      .SetActorType(typeof(StatelessActor))
      .SetIsExplicitVirtual(false)
      .SetFactoryType(typeof(IStatelessActorFactory))
      .SetConstructor(constructorInfo)
      .SetStateFields()
      .SetImplementations(_ => _
        .Add(_ => _
          .SetIsDefault(true)
          .SetImplementationType(typeof(StatelessActor))
          .SetConstructor(constructorInfo))));
  }
}
