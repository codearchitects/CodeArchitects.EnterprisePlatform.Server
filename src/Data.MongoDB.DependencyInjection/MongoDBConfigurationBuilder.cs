using CodeArchitects.Platform.Data.MongoDB.Model;
using CodeArchitects.Platform.Data.MongoDB.Model.Implementation;
using CodeArchitects.Platform.Data.MongoDB.Query;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace CodeArchitects.Platform.Data.MongoDB.DependencyInjection;

internal class MongoDBConfigurationBuilder : IMongoDBConfigurationBuilder, IMongoDBConfigurationBuilderWithClient, IMongoDBConfigurationBuilderWithDatabase
{
  private readonly HashSet<Assembly> _entityAssemblies;
  private MongoClient? _client;
  private string? _databaseName;
  private MongoDatabaseSettings? _settings;
  private Type? _seedType;

  public MongoDBConfigurationBuilder()
  {
    _entityAssemblies = new();
  }

  public void AddServices(IServiceCollection services)
  {
    if (_client is null)
      throw new InvalidOperationException("MongoClient was not configured.");
    if (_databaseName is null)
      throw new InvalidOperationException("Database was not configured.");

    DataModel dataModel = new();
    IEnumerable<Type> entityTypes = _entityAssemblies
      .SelectMany(assebly => assebly.GetTypes())
      .Where(type => type.IsDefined(typeof(TableAttribute), inherit: true));

    foreach (Type entityType in entityTypes)
    {
      dataModel.AddEntity(entityType);
    }

    if (_seedType is not null)
    {
      services.AddScoped(typeof(DataSeed), _seedType);
    }

    services.AddSingleton<IPredicateTemplateProvider, PredicateTemplateProvider>();
    services.AddSingleton<IPredicateProvider, PredicateProvider>();

    services.AddSingleton<IDataModel>(dataModel);

    services.AddSingleton<IMongoClient>(_client);
    services.AddScoped(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(_databaseName, _settings));

    services.AddScoped<StateManager>();
    services.AddScoped<IDataContext, DataContext>();
    services.AddScoped<IStateManager>(sp => sp.GetRequiredService<StateManager>());
    services.AddScoped<IUnitOfWorkManager>(sp => sp.GetRequiredService<StateManager>());
    services.AddScoped(sp => sp.GetRequiredService<IUnitOfWorkManager>().Begin());
  }

  public IMongoDBConfigurationBuilderWithDatabase AddEntitiesFrom(Assembly assembly)
  {
    _entityAssemblies.Add(assembly);
    return this;
  }

  public IMongoDBConfigurationBuilderWithClient UseClient(MongoClient client)
  {
    _client = client;
    return this;
  }

  public IMongoDBConfigurationBuilderWithClient UseConnectionString(string connectionString)
  {
    _client = new MongoClient(connectionString);
    return this;
  }

  public IMongoDBConfigurationBuilderWithDatabase UseDatabase(string databaseName, MongoDatabaseSettings? settings = null)
  {
    _databaseName = databaseName;
    _settings = settings;
    return this;
  }

  public IMongoDBConfigurationBuilder UseSeed(Type seedType)
  {
    if (seedType is null)
      throw new ArgumentNullException(nameof(seedType));

    if (!seedType.IsSubclassOf(typeof(DataSeed)))
      throw new ArgumentException($"Type '{seedType}' does not extend '{nameof(DataSeed)}'.");

    _seedType = seedType;
    return this;
  }
}
