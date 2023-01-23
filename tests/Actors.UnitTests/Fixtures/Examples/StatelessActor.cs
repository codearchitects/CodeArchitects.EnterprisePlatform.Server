using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.FluentMock;
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

  static StatelessActorFixture()
  {
    ConstructorInfo constructor = typeof(StatelessActor).GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(IService1) });

    ParameterInfo[] constructorParameters = constructor.GetParameters();

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
      .SetCategoryIndex(0));

    IImplementationDescriptor implementation = ImplementationDescriptorBuilder.Build(_ => _
      .SetType(typeof(StatelessActor))
      .SetConstructor(_ => _
        .SetConstructor(constructor)
        .SetDependencies(service1Dependency)
        .SetContextDependency(null as IContextDependencyDescriptor)
        .SetServiceDependencies(service1Dependency)
        .SetStateDependencies()));

    Descriptor = ActorDescriptorBuilder.Build(_ => _
      .SetInterfaceType(typeof(IStatelessActor))
      .SetImplementationType(typeof(StatelessActor))
      .SetIsVirtual(true)
      .SetStartingImplementation(implementation)
      .SetImplementations(implementation)
      .SetId(_ => _
        .SetIdType(typeof(string))
        .SetHasIdSource(false)
        .SetStateDependency(null as IStateDependencyDescriptor)
        .SetStateProperty(null))
      .SetState(_ => _
        .SetStateType(typeof(NoState))
        .SetIsStateless(true)
        .SetDefaultComponents())
      .SetFactory(_ => _
        .SetFactoryType(typeof(IStatelessActorFactory))
        .SetCreateAsyncMethod(null)
        .SetGetMethod(factoryGetMethod))
      .SetMethods());
  }
}
