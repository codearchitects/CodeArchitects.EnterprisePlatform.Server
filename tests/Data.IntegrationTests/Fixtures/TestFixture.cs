using CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.Data.EntityFrameworkCore;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Features.SoftDelete;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Materialization;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Query;
using CodeArchitects.Platform.Data.Fixtures.Model;
using Docker.DotNet;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Respawn;
using Xunit.Abstractions;

namespace CodeArchitects.Platform.Data.Fixtures;

public class TestFixture : IAsyncLifetime
{
  private readonly MsSqlTestcontainer _msSqlContainer;
  private readonly PostgreSqlTestcontainer _postgresContainer;

  private readonly TestLocalData _localData;

  private readonly Lazy<DbContextOptions> _sqlServerOptionsLazy;
  private readonly Lazy<DbContextOptions> _postgresOptionsLazy;
  private readonly Lazy<TestDbContext> _sqlServerDbContextLazy;
  private readonly Lazy<TestDbContext> _postgresDbContextLazy;

  private readonly Lazy<SqlConnection> _sqlServerConnectionLazy;
  private readonly Lazy<NpgsqlConnection> _postgresConnectionLazy;

  private Respawner _sqlServerRespawner = default!;
  private Respawner _postgresRespawner = default!;

  private TestDbContext? _dbContext;

  public TestFixture()
  {
    _msSqlContainer = new TestcontainersBuilder<MsSqlTestcontainer>()
      .WithDatabase(new MsSqlTestcontainerConfiguration
      {
        Database = "Test",
        Password = "Password1"
      })
      .Build();

    _postgresContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
      .WithDatabase(new PostgreSqlTestcontainerConfiguration
      {
        Database = "Test",
        Username = "codearchitects",
        Password = "Password1"
      })
      .Build();

    _localData = new();

    _sqlServerOptionsLazy = new(() => new DbContextOptionsBuilder()
      .UseSqlServer(SqlServerConnectionString)
      .EnableSensitiveDataLogging()
      .UseLoggerFactory(new XunitLoggerFactory(_localData))
      .UseData(data => data
        .UseMultitenancy(new MultitenancyDescriptor(MultitenancyContext))
        .UseSoftDelete(new SoftDeleteDescriptor(SoftDeleteContext)))
      .Options);

    _postgresOptionsLazy = new(() => new DbContextOptionsBuilder()
      .UseNpgsql(PostgresConnectionString)
      .EnableSensitiveDataLogging()
      .UseLoggerFactory(new XunitLoggerFactory(_localData))
      .UseData(data => data
        .UseMultitenancy(new MultitenancyDescriptor(MultitenancyContext))
        .UseSoftDelete(new SoftDeleteDescriptor(SoftDeleteContext)))
      .Options);

    _sqlServerDbContextLazy = new(() => new TestDbContext(_sqlServerOptionsLazy.Value));

    _postgresDbContextLazy = new(() => new TestDbContext(_postgresOptionsLazy.Value));

    _sqlServerConnectionLazy = new(() => new SqlConnection(SqlServerConnectionString));

    _postgresConnectionLazy = new(() => new NpgsqlConnection(PostgresConnectionString));
  }

  public TestDbContext DbContext => _dbContext ?? throw new InvalidOperationException($"Cannot access {nameof(DbContext)} before calling {nameof(CreateRepository)}.");

  public MultitenancyContext MultitenancyContext => _localData.MultitenancyContext;

  public SoftDeleteContext SoftDeleteContext => _localData.SoftDeleteContext;

  private string SqlServerConnectionString => _msSqlContainer.ConnectionString + "TrustServerCertificate=True;";

  private string PostgresConnectionString => _postgresContainer.ConnectionString;

  public IRepository<TEntity, TKey> CreateRepository<TEntity, TKey>(
    RepositoryDependencies dependencies,
    IEnumerable<object>? seed = null)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    var (implementation, provider, trackingContext) = dependencies;

    _dbContext = provider switch
    {
      DatabaseProvider.SqlServer => _sqlServerDbContextLazy.Value,
      DatabaseProvider.Postgres  => _postgresDbContextLazy.Value,
      _                          => throw Errors.Unreacheable
    };

    if (seed is not null)
    {
      _dbContext.AddRange(seed);
      _dbContext.SaveChanges();
    }

    switch (implementation)
    {
      case RepositoryImplementation.AdoNet:
        throw new NotSupportedException();
      case RepositoryImplementation.EntityFrameworkCore:
        StateManager<TestDbContext> contextManager = new(_dbContext);

        PredicateTemplateFactory templateFactory = new(_dbContext.Model);
        PredicateProvider predicateProvider = new(templateFactory, PredicateTemplateCache.Create());

        DefaultEntityFactoryFactory entityFactoryFactory = new(_dbContext.Model);
        DefaultEntityFactory entityFactory = new(entityFactoryFactory, DefaultEntityFactoryCache.Create());

        DataContext<TestDbContext> context = new(contextManager, trackingContext, predicateProvider, entityFactory);

        return new EFCoreRepository<TEntity, TKey>(context);
      default:
        throw Errors.Unreacheable;
    }
  }

  public Task SetupAsync(ITestOutputHelper output)
  {
    _localData.Initialize(output);

    return Task.CompletedTask;
  }

  public async Task ResetAsync()
  {
    await _sqlServerConnectionLazy.Value.OpenAsync();
    await _sqlServerRespawner.ResetAsync(_sqlServerConnectionLazy.Value);
    await _sqlServerConnectionLazy.Value.CloseAsync();

    await _postgresConnectionLazy.Value.OpenAsync();
    await _postgresRespawner.ResetAsync(_postgresConnectionLazy.Value);
    await _postgresConnectionLazy.Value.CloseAsync();

    DbContext.ChangeTracker.Clear();
    _dbContext = null;
    _localData.Reset();
  }

  public void SoftDelete(SoftDeleteEntity entity)
  {
    bool shouldFilter = SoftDeleteContext.ShouldFilter;
    SoftDeleteContext.ShouldFilter = false;
    DbContext.Attach(entity).Property(SoftDeleteEntity.SoftDeletePropertyName).CurrentValue = true;
    DbContext.SaveChanges();
    SoftDeleteContext.ShouldFilter = shouldFilter;
  }

  async Task IAsyncLifetime.InitializeAsync()
  {
    if (OperatingSystem.IsLinux())
    {
      DockerClient dockerClient = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient();

      await dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
      {
        FromImage = "testcontainers/ryuk",
        Tag = "0.3.4"
      }, null, new Progress<JSONMessage>());
    }

    await _msSqlContainer.StartAsync();
    await _postgresContainer.StartAsync();

    await InitializeDatabaseAsync(_sqlServerDbContextLazy.Value);
    await InitializeDatabaseAsync(_postgresDbContextLazy.Value);

    await _sqlServerConnectionLazy.Value.OpenAsync();
    _sqlServerRespawner = await Respawner.CreateAsync(_sqlServerConnectionLazy.Value, new()
    {
      DbAdapter = DbAdapter.SqlServer
    });
    await _sqlServerConnectionLazy.Value.CloseAsync();

    await _postgresConnectionLazy.Value.OpenAsync();
    _postgresRespawner = await Respawner.CreateAsync(_postgresConnectionLazy.Value, new()
    {
      DbAdapter = DbAdapter.Postgres
    });
    await _postgresConnectionLazy.Value.CloseAsync();
  }

  async Task IAsyncLifetime.DisposeAsync()
  {
    await CleanUpContainerAsync(_msSqlContainer);
    await CleanUpContainerAsync(_postgresContainer);

    _sqlServerDbContextLazy.Value.Dispose();
    _postgresDbContextLazy.Value.Dispose();

    _sqlServerConnectionLazy.Value.Dispose();
    _postgresConnectionLazy.Value.Dispose();
  }

  private static async Task InitializeDatabaseAsync(TestDbContext context)
  {
    await context.Database.EnsureCreatedAsync();
  }

  private static async Task CleanUpContainerAsync(TestcontainersContainer container)
  {
    await container.StopAsync();
    await container.CleanUpAsync();
    await container.DisposeAsync();
  }
}
