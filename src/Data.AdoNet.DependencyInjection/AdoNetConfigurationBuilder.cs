using CodeArchitects.Platform.Data.AdoNet.Interceptors;
using CodeArchitects.Platform.Data.Features.Concurrency;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.DependencyInjection;

internal class AdoNetConfigurationBuilder : IAdoNetConfigurationBuilder, IAdoNetConfigurationBuilderWithProvider
{
  private readonly List<Type> _commandInterceptorTypes;
  private DatabaseProvider? _provider;
  private Type? _modelConfigurationType;

  public AdoNetConfigurationBuilder()
  {
    _commandInterceptorTypes = new();
    ConcurrencyTokenProviderDescriptor = new(typeof(IConcurrencyTokenProvider), typeof(ConcurrencyTokenProvider), ServiceLifetime.Singleton);
  }

  public DatabaseProvider Provider => _provider ?? throw new InvalidOperationException("No database provider was specified.");

  public Type ModelConfigurationType => _modelConfigurationType ?? throw new InvalidOperationException("No model configuration was provided.");

  public IReadOnlyCollection<Type> CommandInterceptorTypes => _commandInterceptorTypes;

  public Type? SeedType { get; private set; }

  public ServiceDescriptor ConcurrencyTokenProviderDescriptor { get; private set; }

  public IAdoNetConfigurationBuilderWithProvider UseProvider<TProvider>(Action<TProvider> configureAction)
    where TProvider : DatabaseProvider, new()
  {
    if (configureAction is null)
      throw new ArgumentNullException(nameof(configureAction));
    if (_provider is not null)
      throw new InvalidOperationException("Cannot specify multiple database provider.");

    TProvider provider = new();
    configureAction(provider);

    _provider = provider;

    return this;
  }

  public IAdoNetConfigurationBuilderWithProvider AddCommandInterceptor(Type interceptorType)
  {
    if (interceptorType is null)
      throw new ArgumentNullException(nameof(interceptorType));

    Type interfaceType = Provider.MakeGenericType(typeof(ICommandInterceptor<>));
    if (!interfaceType.IsAssignableFrom(interceptorType))
      throw new ArgumentException($"Type '{interceptorType.Name}' does not implement '{interfaceType.Name}'.");

    _commandInterceptorTypes.Add(interceptorType);
    return this;
  }

  public IAdoNetConfigurationBuilderWithProvider UseModel(Type modelConfigurationType)
  {
    if (modelConfigurationType is null)
      throw new ArgumentNullException(nameof(modelConfigurationType));

    if (!modelConfigurationType.IsSubclassOf(typeof(ModelConfiguration)))
      throw new ArgumentException($"Type '{modelConfigurationType}' does not extend '{nameof(ModelConfiguration)}'.");

    _modelConfigurationType = modelConfigurationType;
    return this;
  }

  public IAdoNetConfigurationBuilderWithProvider UseSeed(Type seedType)
  {
    if (seedType is null)
      throw new ArgumentNullException(nameof(seedType));

    if (!seedType.IsSubclassOf(typeof(DataSeed)))
      throw new ArgumentException($"Type '{seedType}' does not extend '{nameof(DataSeed)}'.");

    SeedType = seedType;
    return this;
  }

  public IAdoNetConfigurationBuilderWithProvider UseConcurrencyTokenProvider(Type tokenProviderType, ServiceLifetime tokenProviderLifetime = ServiceLifetime.Singleton)
  {
    if (tokenProviderType is null)
      throw new ArgumentNullException(nameof(tokenProviderType));
    if (typeof(IConcurrencyTokenProvider).IsAssignableFrom(tokenProviderType) || !tokenProviderType.IsClass || tokenProviderType.IsAbstract)
      throw new ArgumentException($"'{nameof(tokenProviderType)}' should be a concrete class assignable to '{nameof(IConcurrencyTokenProvider)}'.", nameof(tokenProviderType));

    ConcurrencyTokenProviderDescriptor = new ServiceDescriptor(typeof(IConcurrencyTokenProvider), tokenProviderType, tokenProviderLifetime);
    return this;
  }

  public IAdoNetConfigurationBuilderWithProvider UseConcurrencyTokenProvider(IConcurrencyTokenProvider tokenProvider)
  {
    if (tokenProvider is null)
      throw new ArgumentNullException(nameof(tokenProvider));

    ConcurrencyTokenProviderDescriptor = new ServiceDescriptor(typeof(IConcurrencyTokenProvider), tokenProvider);
    return this;
  }

  public IAdoNetConfigurationBuilderWithProvider ScanAssemblyForServices(Assembly assembly, AdoNetServiceTypes serviceTypes = AdoNetServiceTypes.All)
  {
    if (assembly is null)
      throw new ArgumentNullException(nameof(assembly));

    Type[] types = assembly.GetTypes();

    if (serviceTypes.HasFlag(AdoNetServiceTypes.ModelConfiguration))
    {
      AddModelConfiguration();
    }

    if (serviceTypes.HasFlag(AdoNetServiceTypes.CommandInterceptors))
    {
      AddCommandInterceptors();
    }

    if (serviceTypes.HasFlag(AdoNetServiceTypes.DataSeed))
    {
      AddDataSeed();
    }

    if (serviceTypes.HasFlag(AdoNetServiceTypes.ConcurrencyTokenProvider))
    {
      AddConcurrencyTokenProvider();
    }

    return this;

    void AddModelConfiguration()
    {
      Type? modelConfigurationType;
      try
      {
        modelConfigurationType = types.SingleOrDefault(type => !type.IsAbstract && typeof(ModelConfiguration).IsAssignableFrom(type));
      }
      catch (InvalidOperationException)
      {
        throw new InvalidOperationException($"Found more than one class extending '{nameof(ModelConfiguration)}' in the provided assembly.");
      }

      _modelConfigurationType = modelConfigurationType
        ?? throw new InvalidOperationException($"A class extending '{nameof(ModelConfiguration)}' was not found in the provided assembly.");
    }

    void AddCommandInterceptors()
    {
      Type commandInterceptorInterfaceType = Provider.MakeGenericType(typeof(ICommandInterceptor<>));
      _commandInterceptorTypes.AddRange(types.Where(type => !type.IsAbstract && commandInterceptorInterfaceType.IsAssignableFrom(type)));
    }

    void AddDataSeed()
    {
      Type? seedType;
      try
      {
        seedType = types.SingleOrDefault(type => !type.IsAbstract && typeof(DataSeed).IsAssignableFrom(type));
      }
      catch (InvalidOperationException)
      {
        throw new InvalidOperationException($"Found more than one class extending '{nameof(DataSeed)}' in the provided assembly.");
      }

      SeedType = seedType;
    }

    void AddConcurrencyTokenProvider()
    {
      Type? tokenProviderType;
      try
      {
        tokenProviderType = types.SingleOrDefault(type => !type.IsAbstract && type.IsClass && typeof(IConcurrencyTokenProvider).IsAssignableFrom(type));
      }
      catch (InvalidOperationException)
      {
        throw new InvalidOperationException($"Found more than one class implementing '{nameof(IConcurrencyTokenProvider)}' in the provided assembly.");
      }

      ConcurrencyTokenProviderDescriptor = new ServiceDescriptor(typeof(IConcurrencyTokenProvider), tokenProviderType, ServiceLifetime.Singleton);
    }
  }
}
