using CodeArchitects.Platform.Data.MongoDB.Collections;
using CodeArchitects.Platform.Data.MongoDB.Filters;
using CodeArchitects.Platform.Data.MongoDB.Model;
using CodeArchitects.Platform.Data.MongoDB.Model.Implementation;
using CodeArchitects.Platform.Data.MongoDB.Serialization;
using CodeArchitects.Platform.Data.MongoDB.Transactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System.Reflection;

namespace CodeArchitects.Platform.Data.MongoDB.DependencyInjection;

internal class MongoDBConfigurationBuilder :
  IMongoDBConfigurationBuilder,
  IMongoDBConfigurationBuilderWithClient,
  IMongoDBConfigurationBuilderWithDatabase
{
  private readonly DataModelBuilder _modelBuilder = new();
  private readonly List<Action<ConventionPack>> _conventionConfigurators = new();
  private readonly MongoDBOptions _options = new();

  private readonly List<Type> _seedTypes = new();

  private Func<IServiceProvider, IMongoClient>? _clientFactory;

  #region Client

  public IMongoDBConfigurationBuilderWithClient UseConnectionString(string connectionString)
  {
    if (string.IsNullOrWhiteSpace(connectionString))
      throw new ArgumentException("The MongoDB connection string cannot be empty.", nameof(connectionString));

    MongoClientSettings settings = MongoClientSettings.FromConnectionString(connectionString);
    return UseClient(_ => new MongoClient(settings));
  }

  public IMongoDBConfigurationBuilderWithClient UseConnectionString(string connectionString, Action<MongoClientSettings> configure)
  {
    if (string.IsNullOrWhiteSpace(connectionString))
      throw new ArgumentException("The MongoDB connection string cannot be empty.", nameof(connectionString));
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));

    MongoClientSettings settings = MongoClientSettings.FromConnectionString(connectionString);
    configure(settings);

    return UseClient(_ => new MongoClient(settings));
  }

  public IMongoDBConfigurationBuilderWithClient UseClient(IMongoClient client)
  {
    if (client is null)
      throw new ArgumentNullException(nameof(client));

    return UseClient(_ => client);
  }

  public IMongoDBConfigurationBuilderWithClient UseClient(Func<IServiceProvider, IMongoClient> factory)
  {
    _clientFactory = factory ?? throw new ArgumentNullException(nameof(factory));
    return this;
  }

  #endregion

  #region Database

  public IMongoDBConfigurationBuilderWithDatabase UseDatabase(string databaseName, MongoDatabaseSettings? settings = null)
  {
    if (string.IsNullOrWhiteSpace(databaseName))
      throw new ArgumentException("The MongoDB database name cannot be empty.", nameof(databaseName));

    _options.DatabaseName = databaseName;
    _options.DatabaseSettings = settings;
    return this;
  }

  #endregion

  #region Model and behaviour

  public IMongoDBConfigurationBuilderWithDatabase AddEntitiesFrom(Assembly assembly, Func<Type, bool>? filter = null)
  {
    _modelBuilder.AddEntitiesFrom(assembly, filter);
    return this;
  }

  public IMongoDBConfigurationBuilderWithDatabase AddEntity(Type entityType)
  {
    _modelBuilder.AddEntity(entityType);
    return this;
  }

  public IMongoDBConfigurationBuilderWithDatabase UseTransactions(TransactionMode mode, TransactionOptions? options = null)
  {
    _options.TransactionMode = mode;
    _options.TransactionOptions = options;
    return this;
  }

  public IMongoDBConfigurationBuilderWithDatabase UseGuidRepresentation(GuidRepresentation representation)
  {
    _options.GuidRepresentation = representation;
    return this;
  }

  public IMongoDBConfigurationBuilderWithDatabase ConfigureConventions(Action<ConventionPack> configure)
  {
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));

    _conventionConfigurators.Add(configure);
    return this;
  }

  public IMongoDBConfigurationBuilderWithDatabase UseSeed(Type seedType)
  {
    if (seedType is null)
      throw new ArgumentNullException(nameof(seedType));

    if (!seedType.IsSubclassOf(typeof(DataSeed)))
      throw new ArgumentException($"Type '{seedType}' does not extend '{nameof(DataSeed)}'.");

    if (!_seedTypes.Contains(seedType))
    {
      _seedTypes.Add(seedType);
    }

    return this;
  }

  public IMongoDBConfigurationBuilderWithDatabase AddSeedsFrom(Assembly assembly)
  {
    if (assembly is null)
      throw new ArgumentNullException(nameof(assembly));

    foreach (Type type in assembly.GetTypes())
    {
      if (type.IsClass && !type.IsAbstract && !type.IsGenericTypeDefinition && type.IsSubclassOf(typeof(DataSeed)))
      {
        UseSeed(type);
      }
    }

    return this;
  }

  #endregion

  public void AddServices(IServiceCollection services)
  {
    // 1. Configuration validation. Everything verifiable without touching the server fails here,
    //    while the stack trace still points at the composition code.
    if (_clientFactory is null)
      throw new InvalidOperationException(
        "The MongoDB client was not configured. Use UseConnectionString(connectionString) or UseClient(client).");

    if (string.IsNullOrWhiteSpace(_options.DatabaseName))
      throw new InvalidOperationException("The MongoDB database was not configured. Use UseDatabase(name).");

    // 2. Model construction and validation: empty model, unresolvable key, collection collisions.
    DataModel dataModel = _modelBuilder.Build();

    // 3. Global serialization setup: must run before any document is serialized, once per process.
    MongoDBSerializationInitializer.EnsureInitialized(_options.GuidRepresentation, _conventionConfigurators);

    // 4. Service registrations.
    Func<IServiceProvider, IMongoClient> clientFactory = _clientFactory;

    services.AddSingleton(_options);
    services.AddSingleton<IDataModel>(dataModel);
    services.AddSingleton<IFilterProvider, FilterProvider>();

    services.AddSingleton(clientFactory);
    // IMongoDatabase is thread-safe and immutable: a singleton lets CollectionProvider
    // cache collections for the lifetime of the application instead of per request.
    services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>()
      .GetDatabase(_options.DatabaseName, _options.DatabaseSettings));
    services.AddSingleton<ICollectionProvider, CollectionProvider>();
    services.AddSingleton<ITransactionCapabilityProbe>(sp => new TransactionCapabilityProbe(
      sp.GetRequiredService<IMongoDatabase>(),
      Logger<TransactionCapabilityProbe>(sp)));

    // Explicit factory: ILogger<T> is only registered when the application calls AddLogging,
    // so fall back to NullLogger instead of making the whole registration fail.
    services.AddScoped(sp => new StateManager(
      sp.GetRequiredService<IMongoClient>(),
      sp.GetRequiredService<ITransactionCapabilityProbe>(),
      sp.GetRequiredService<MongoDBOptions>(),
      Logger<StateManager>(sp)));
    services.AddScoped<IStateManager>(sp => sp.GetRequiredService<StateManager>());
    services.AddScoped<IUnitOfWorkManager>(sp => sp.GetRequiredService<StateManager>());
    services.AddScoped(sp => sp.GetRequiredService<IUnitOfWorkManager>().Begin());

    services.AddScoped<DataContext>();
    services.AddScoped<IDataContext>(sp => sp.GetRequiredService<DataContext>());
    // The provider-agnostic contract must resolve too: generated code depends on it.
    services.AddScoped<Data.IDataContext>(sp => sp.GetRequiredService<DataContext>());

    services.AddScoped<Seeder>();
    services.AddScoped<ISeeder>(sp => sp.GetRequiredService<Seeder>());

    // One registration per seed: GetServices<DataSeed>() then returns them all.
    foreach (Type seedType in _seedTypes)
    {
      services.AddScoped(typeof(DataSeed), seedType);
    }
  }

  private static ILogger<T> Logger<T>(IServiceProvider services)
  {
    ILoggerFactory? factory = services.GetService<ILoggerFactory>();
    return factory is null ? NullLogger<T>.Instance : new Logger<T>(factory);
  }
}
