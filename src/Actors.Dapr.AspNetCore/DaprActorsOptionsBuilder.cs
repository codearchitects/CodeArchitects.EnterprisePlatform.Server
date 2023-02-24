using CodeArchitects.Platform.Actors.Dapr.Factory;
using CodeArchitects.Platform.Actors.Dapr.Infrastructure;
using CodeArchitects.Platform.Actors.Dapr.Messaging;
using CodeArchitects.Platform.Actors.Dapr.Proxy;
using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.Builder;
using CodeArchitects.Platform.Actors.Descriptors.Factory;
using CodeArchitects.Platform.Actors.Descriptors.Reflection;
using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.Messaging;
using CodeArchitects.Platform.Actors.Scheduling;
using CodeArchitects.Platform.Emit;
using CodeArchitects.Platform.Emit.Reflection;
using Dapr.Actors.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Dapr.AspNetCore;

internal class DaprActorsOptionsBuilder : IDaprActorsOptionsBuilder
{
  private readonly HashSet<Assembly> _assemblies;
  private readonly HashSet<Type> _actorTypes;
  private readonly Dictionary<Type, ActorRuntimeOptions> _specificRuntimeOptions;
  private Action<ActorRuntimeOptions>? _configureRuntimeOptions;
  private Type? _actorConfigurationType;
  private bool _ignoreAccessChecks;

  public DaprActorsOptionsBuilder()
  {
    _assemblies = new();
    _actorTypes = new();
    _specificRuntimeOptions = new();
  }

  public void RegisterServices(IServiceCollection services, IConfiguration configuration)
  {
    bool useReflection = _assemblies.Count > 0 || _actorTypes.Count > 0;
    bool useBuilder = _actorConfigurationType is not null;

    if (!useReflection && !useBuilder)
      throw new InvalidOperationException("Neither an actor configuration class was provided, nor actors were registered via reflection.");
    if (useReflection && useBuilder)
      throw new InvalidOperationException($"A combination of actor registrations were used. Either use the '{nameof(UseActorConfiguration)}' method to specify a configuration class or use the '{nameof(AddActor)}' and/or '{nameof(ScanAssembly)}' methods to register actors via reflection.");

    ActorModelFactory factory;
    if (useBuilder)
    {
      ActorConfiguration actorConfiguration;

      ConstructorInfo? configurationConstructor = _actorConfigurationType!.GetConstructor(
        bindingAttr: BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
        binder: null,
        types: new[] { typeof(IConfiguration) },
        modifiers: null);
      ConstructorInfo? parameterlessConstructor = _actorConfigurationType.GetConstructor(
        bindingAttr: BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
        binder: null,
        types: Type.EmptyTypes,
        modifiers: null);

      actorConfiguration =
        configurationConstructor is not null ? (ActorConfiguration)configurationConstructor.Invoke(new object?[] { configuration })! :
        parameterlessConstructor is not null ? (ActorConfiguration)parameterlessConstructor.Invoke(Array.Empty<object?>()) :
        throw new InvalidOperationException($"The actor configuration type must have a parameterless constructor or a constructor which accepts an '{nameof(IConfiguration)}' instance.");

      actorConfiguration.Configure();
      factory = actorConfiguration;
    }
    else
    {
      ReflectionMetadataContext context = new();
      foreach (Assembly assembly in _assemblies)
      {
        context.AddAssembly(assembly);
      }
      foreach (Type actorType in _actorTypes)
      {
        context.AddActor(actorType);
      }

      factory = context;
    }

    IActorModel model = factory.CreateModel(DynamicAssembly.Module);

    services.AddScoped(typeof(IManagerFactory<,>), typeof(ManagerFactory<,>));
    services.AddSingleton(model);

    HashSet<Assembly> actorAssemblies = new();
    List<Action<ActorRuntimeOptions>> actorRegistrations = new();

    ReflectionILGeneratorProvider ilProvider = new();
    ActorHostTypeBuilder actorHostTypeBuilder = new(DynamicAssembly.Module, ilProvider);
    ActorProxyTypeBuilder actorProxyTypeBuilder = new(DynamicAssembly.Module, ilProvider);
    DaprProxyFactoryTypeBuilder proxyFactoryTypeBuilder = new(DynamicAssembly.Module, ilProvider);
    DaprMessageHandlerTypeBuilder messageHandlerTypeBuilder = new(DynamicAssembly.Module, ilProvider);

    foreach (IActorDescriptor actor in model.Actors)
    {
      Assembly actorAssembly = actor.ActorType.Assembly;

      if (!actorAssemblies.Contains(actorAssembly))
      {
        if (_ignoreAccessChecks)
        {
          DynamicAssembly.Module.IgnoreAccessChecksTo(actorAssembly.GetName().Name!);
        }

        actorAssemblies.Add(actorAssembly);
      }

      ActorHostEmitResult hostEmitResult = actorHostTypeBuilder.Build(actor, null);
      Type proxyType = actorProxyTypeBuilder.Build(actor, hostEmitResult);
      Type actorFactoryType = proxyFactoryTypeBuilder.Build(actor, hostEmitResult, proxyType, null);

      Type activityManagerType = typeof(IActivityManager<>).MakeGenericType(actor.ActorType);
      object activityManager = CreateActivityManager(actor);

      services.AddSingleton(activityManagerType, activityManager);
      services.AddSingleton(actor.Factory.FactoryType, actorFactoryType);

      if (hostEmitResult.HandlerInterfaceType is Type handlerInterfaceType)
      {
        Type handlerType = messageHandlerTypeBuilder.Build(actor, handlerInterfaceType, null);
        ActorMessaging.MessagingAssembly.AddHandlerType(handlerType);
      }

      actorRegistrations.Add(delegate (ActorRuntimeOptions runtimeOptions)
      {
        runtimeOptions.Actors.Add(new ActorRegistration(ActorTypeInformation.Get(hostEmitResult.ClassType, actor.ActorType.Name)));
      });
    }

    services.AddActors(runtimeOptions =>
    {
      _configureRuntimeOptions?.Invoke(runtimeOptions);

      foreach (Action<ActorRuntimeOptions> registerActor in actorRegistrations)
      {
        registerActor(runtimeOptions);
      }
    });
  }

  public IDaprActorsOptionsBuilder AccessPrivates(bool accessPrivates = true)
  {
    _ignoreAccessChecks = accessPrivates;
    return this;
  }

  public IDaprActorsOptionsBuilder ConfigureRuntimeOptions(Action<ActorRuntimeOptions> configure)
  {
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));

    _configureRuntimeOptions = configure;
    return this;
  }

  public IDaprActorsOptionsBuilder ScanAssembly(Assembly assembly)
  {
    if (assembly is null)
      throw new ArgumentNullException(nameof(assembly));

    _assemblies.Add(assembly);
    return this;
  }

  public IDaprActorsOptionsBuilder AddActor(Type actorType)
  {
    if (actorType is null)
      throw new ArgumentNullException(nameof(actorType));

    _actorTypes.Add(actorType);
    return this;
  }

  public IDaprActorsOptionsBuilder UseActorConfiguration(Type actorConfigurationType)
  {
    if (actorConfigurationType is null)
      throw new ArgumentNullException(nameof(actorConfigurationType));

    if (!typeof(ActorConfiguration).IsAssignableFrom(actorConfigurationType))
      throw new ArgumentException($"The configuration type must inherit from '{nameof(ActorConfiguration)}'.", nameof(actorConfigurationType));

    if (_actorConfigurationType is not null)
      throw new InvalidOperationException("An actor configuration type was already specified.");

    _actorConfigurationType = actorConfigurationType;
    return this;
  }

  public IDaprActorsOptionsBuilder UseRuntimeOptions(Type actorType, ActorRuntimeOptions options)
  {
    if (actorType is null)
      throw new ArgumentNullException(nameof(actorType));
    if (options is null)
      throw new ArgumentNullException(nameof(options));

    _specificRuntimeOptions[actorType] = options;
    return this;
  }

  private object CreateActivityManager(IActorDescriptor actor)
  {
    Type activityManagerType = typeof(ActivityManager<>).MakeGenericType(actor.ActorType);

    MethodInfo createMethod = activityManagerType.GetRequiredMethod(
      name: nameof(ActivityManager<object>.Create),
      bindingAttr: BindingFlags.Static | BindingFlags.Public,
      types: new[] { typeof(IActorDescriptor) });

    return createMethod.Invoke(null, new object?[] { actor })!;
  }
}
