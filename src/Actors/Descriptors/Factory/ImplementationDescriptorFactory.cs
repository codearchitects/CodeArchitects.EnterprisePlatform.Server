using CodeArchitects.Platform.Actors.Descriptors.Implementation;
using CodeArchitects.Platform.Actors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Factory;

internal abstract class ImplementationDescriptorFactory<TActor>
  where TActor : class
{
  private static readonly MethodInfo s_getServiceMethod;
  private static readonly MethodInfo s_getRequiredServiceMethod;

  static ImplementationDescriptorFactory()
  {
    s_getServiceMethod = typeof(ServiceProviderServiceExtensions).GetRequiredMethod(
      name: nameof(ServiceProviderServiceExtensions.GetService),
      bindingAttr: BindingFlags.Static | BindingFlags.Public,
      types: new[] { typeof(IServiceProvider) });

    s_getRequiredServiceMethod = typeof(ServiceProviderServiceExtensions).GetRequiredMethod(
      name: nameof(ServiceProviderServiceExtensions.GetRequiredService),
      bindingAttr: BindingFlags.Static | BindingFlags.Public,
      types: new[] { typeof(IServiceProvider) });
  }

  private readonly int _id;
  private readonly ActorDescriptorFactory<TActor> _actorDescriptorFactory;

  protected ImplementationDescriptorFactory(int id, ActorDescriptorFactory<TActor> actorDescriptorFactory)
  {
    _id = id;
    _actorDescriptorFactory = actorDescriptorFactory;
  }

  public abstract bool IsDefault { get; }

  public abstract Type ImplementationType { get; }

  protected abstract bool HasStateFields { get; }

  protected abstract ConstructorInfo? Constructor { get; }

  protected Type ActorType => typeof(TActor);

  public IImplementationDescriptor<TActor, TState> CreateDescriptor<TState>(IReadOnlyList<FieldInfo> stateFields)
    where TState : ActorState
  {
    if (_id != 0 && HasStateFields)
      throw InvalidActorException.StateMustBeDefinedInBaseActor(ActorType, ImplementationType);
    if (ImplementationType.IsAbstract)
      throw InvalidActorException.AbstractImplementation(ImplementationType);

    ConstructorInfo constructor = GetConstructor();
    ImplementationFactory<TActor, TState> implementationFactory = CreateImplementationFactory<TState>(constructor, stateFields);

    return new ImplementationDescriptor<TActor, TState>(_id, implementationFactory, ImplementationType);
  }

  private ConstructorInfo GetConstructor()
  {
    if (Constructor is ConstructorInfo constructor)
      return constructor;

    ConstructorInfo[] constructors = ImplementationType
      .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    if (constructors.Length != 1)
      throw InvalidActorException.AmbiguousActorConstructor(ImplementationType);
    return constructors[0];
  }

  private ImplementationFactory<TActor, TState> CreateImplementationFactory<TState>(ConstructorInfo constructor, IReadOnlyList<FieldInfo> stateFields)
    where TState : ActorState
  {
    ParameterExpression servicesParam = Expression.Parameter(typeof(IServiceProvider), "services");
    ParameterExpression stateParam = Expression.Parameter(typeof(TState), "state");
    ParameterExpression contextParam = Expression.Parameter(typeof(IActorContext<TActor>), "context");

    int stateComponentCount = 0;
    List<Expression> arguments = new List<Expression>();
    foreach (ParameterInfo parameter in constructor.GetParameters())
    {
      string? parameterName = parameter.Name;
      if (parameterName is null)
        throw InvalidActorException.StateComponentsMismatch(ImplementationType);

      Type parameterType = parameter.ParameterType;

      if (_actorDescriptorFactory.TryGetStateComponent(parameterName, out StateComponentMetadata<TActor>? component))
      {
        FieldInfo stateField = stateFields[component.Index];

        arguments.Add(Expression.Field(stateParam, stateField));
        stateComponentCount++;
        continue;
      }

      if (typeof(IActorContext).IsAssignableFrom(parameterType))
      {
        CheckActorContext(parameterType, parameter.Name);

        arguments.Add(contextParam);
        continue;
      }

      MethodInfo getServiceMethod = (parameter.HasDefaultValue
        ? s_getServiceMethod
        : s_getRequiredServiceMethod).MakeGenericMethod(parameterType);

      arguments.Add(Expression.Call(getServiceMethod, servicesParam));
    }

    if (stateComponentCount != _actorDescriptorFactory.StateComponentCount)
      throw InvalidActorException.StateComponentsMismatch(ImplementationType);

    return Expression.Lambda<ImplementationFactory<TActor, TState>>(
      body: Expression.New(constructor, arguments),
      parameters: new[] { servicesParam, stateParam, contextParam })
      .Compile();
  }

  private void CheckActorContext(Type parameterType, string? parameterName)
  {
    if (!parameterType.IsGenericType)
      return;

    Debug.Assert(parameterType.GetGenericTypeDefinition() == typeof(IActorContext<>), "Unexpected actor context type.");

    if (parameterType.GetGenericArguments()[0] != ActorType)
      throw InvalidActorException.WrongGenericActorContext(ImplementationType, parameterName);
  }
}
