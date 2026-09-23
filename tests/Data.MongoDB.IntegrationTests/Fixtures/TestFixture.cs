using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace CodeArchitects.Platform.Data.MongoDB.Fixtures;

/// <summary>
/// Starts two MongoDB deployments: a single-node replica set, which supports transactions, and a
/// standalone server, which does not. The second one is what makes the behaviour of
/// <see cref="TransactionMode"/> verifiable.
/// </summary>
public sealed class TestFixture : IAsyncLifetime
{
  private const string DatabaseName = "customers";
  private const string Image = "mongo:7.0";

  private readonly MongoDbContainer _replicaSet = new MongoDbBuilder()
    .WithImage(Image)
    .WithReplicaSet()
    .Build();

  private readonly MongoDbContainer _standalone = new MongoDbBuilder()
    .WithImage(Image)
    .Build();

  private ServiceProvider _services = default!;
  private ServiceProvider _standaloneServices = default!;

  public async Task InitializeAsync()
  {
    await Task.WhenAll(_replicaSet.StartAsync(), _standalone.StartAsync());

    _services = BuildServices(_replicaSet.GetConnectionString());
    _standaloneServices = BuildServices(_standalone.GetConnectionString());
  }

  public async Task DisposeAsync()
  {
    await _services.DisposeAsync();
    await _standaloneServices.DisposeAsync();
    await _replicaSet.DisposeAsync();
    await _standalone.DisposeAsync();
  }

  private static ServiceProvider BuildServices(string connectionString) =>
    new ServiceCollection()
      .AddData(options => options
        .UseConnectionString(connectionString)
        .UseDatabase(DatabaseName)
        .AddEntitiesFrom(typeof(Customer).Assembly)
        .UseTransactions(TransactionMode.Required))
      .AddScoped<IRepository<Customer, Guid>, MongoDBRepository<Customer, Guid>>()
      .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

  /// <summary>
  /// A scope against the replica set: every test works in its own scope, as a request would.
  /// </summary>
  public TestScope CreateScope() => new(_services.CreateScope());

  /// <summary>
  /// A scope against the standalone server, where transactions are unavailable.
  /// </summary>
  public TestScope CreateStandaloneScope() => new(_standaloneServices.CreateScope());

  /// <summary>
  /// Reads a collection outside any provider scope, to assert on what was really persisted.
  /// </summary>
  public IMongoCollection<TEntity> GetCollection<TEntity>()
    where TEntity : class
  {
    using TestScope scope = CreateScope();
    string name = scope.Context.GetCollection<TEntity>().CollectionNamespace.CollectionName;

    return _services.GetRequiredService<IMongoDatabase>().GetCollection<TEntity>(name);
  }

  /// <summary>
  /// Seeds through the real seeding path: enqueue on the state manager, then commit.
  /// </summary>
  public async Task SeedAsync<TEntity>(IEnumerable<TEntity> entities)
    where TEntity : class
  {
    using TestScope scope = CreateScope();
    await scope.Seeder.ApplyAsync([new InlineSeed<TEntity>(entities)]);
  }

  public async Task ResetAsync()
  {
    IMongoDatabase database = _services.GetRequiredService<IMongoDatabase>();
    IAsyncCursor<string> names = await database.ListCollectionNamesAsync();

    await names.ForEachAsync(collection => database.DropCollectionAsync(collection));
  }

  private sealed class InlineSeed<TEntity>(IEnumerable<TEntity> entities) : DataSeed
    where TEntity : class
  {
    public override void Seed(ISeeder seeder) => seeder.Seed(entities);
  }
}

/// <summary>
/// One dependency-injection scope, with the services a test needs.
/// </summary>
public sealed class TestScope : IDisposable
{
  private readonly IServiceScope _scope;

  internal TestScope(IServiceScope scope) => _scope = scope;

  public IDataContext Context => _scope.ServiceProvider.GetRequiredService<IDataContext>();

  public IUnitOfWorkManager UnitOfWorkManager => _scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

  internal Seeder Seeder => _scope.ServiceProvider.GetRequiredService<Seeder>();

  public TService Get<TService>()
    where TService : notnull
    => _scope.ServiceProvider.GetRequiredService<TService>();

  public MongoDBRepository<TEntity, TKey> Repository<TEntity, TKey>()
    where TEntity : class
    where TKey : IEquatable<TKey>
    => new(Context);

  public IMongoCollection<TEntity> Collection<TEntity>()
    where TEntity : class
    => Context.GetCollection<TEntity>();

  public void Dispose() => _scope.Dispose();
}
