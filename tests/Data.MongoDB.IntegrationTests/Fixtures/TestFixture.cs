using CodeArchitects.Platform.Data.MongoDB.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Mongo2Go;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB.Fixtures;

public sealed class TestFixture : IDisposable
{
  private const string s_databaseName = "customers";

  private readonly MongoDbRunner _runner;
  private readonly IServiceProvider _services;
  private readonly IDataContext _context;

  public TestFixture()
  {
    _runner = MongoDbRunner.Start(singleNodeReplSet: true);

    _services = new ServiceCollection()
      .AddData(options => options
        .UseConnectionString(_runner.ConnectionString)
        .UseDatabase(s_databaseName)
        .AddEntitiesFrom(typeof(Customer).Assembly))
      .BuildServiceProvider();
    _context = _services.GetRequiredService<IDataContext>();
  }

  public MongoDBRepository<TEntity, TKey> CreateRepository<TEntity, TKey>()
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return new MongoDBRepository<TEntity, TKey>(_context);
  }

  public MongoDBRepository<TEntity, TKey> CreateRepository<TEntity, TKey>(IEnumerable<TEntity> seed)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    Seeder seeder = new(_context);
    seeder.Seed(seed);
    return new MongoDBRepository<TEntity, TKey>(_context);
  }

  public IMongoCollection<TEntity> GetCollection<TEntity>()
    where TEntity : class
  {
    return _context.GetCollection<TEntity>();
  }

  public IUnitOfWorkManager GetUnitOfWorkManager()
  {
    return _services.GetRequiredService<IUnitOfWorkManager>();
  }

  public void Dispose()
  {
    _runner.Dispose();
  }

  public async Task ResetAsync()
  {
    IMongoDatabase database = _context.Database;
    IAsyncCursor<string> collectionNames = await database.ListCollectionNamesAsync();
    await collectionNames.ForEachAsync(async collection =>
    {
      await database.DropCollectionAsync(collection);
    });
  }
}
