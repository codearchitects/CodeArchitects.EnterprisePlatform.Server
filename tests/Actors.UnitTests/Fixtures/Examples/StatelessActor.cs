using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.FluentMock;
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
      .SetId(0)
      .SetType(typeof(StatelessActor))
      .SetConstructor(_ => _
        .SetConstructor(s_constructor)
        .SetDependencies(service1Dependency)
        .SetContextDependencies()
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
      .SetIsStateless(true)
      .SetIsVirtual(true)
      .SetId(_ => _
        .SetIdType(typeof(string))
        .SetHasIdSource(false)
        .SetStateDependency(null as IStateDependencyDescriptor)
        .SetStateProperty(null))
      .SetState(_ => _
        .SetType(typeof(NoState))
        .SetStateFields()
        .SetDiscriminatorField(null)
        .SetDefaultValue(NoState.Instance))
      .SetFactory(_ => _
        .SetFactoryType(typeof(IStatelessActorFactory))
        .SetCreateAsyncMethod(null)
        .SetGetMethod(factoryGetMethod)));
  }
}
