using Docker.DotNet;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Data.SqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System.Runtime.InteropServices;
using Xunit.Abstractions;

namespace CodeArchitects.Platform.Data.Fixtures;

public class TestFixture : IAsyncLifetime
{
  private readonly MsSqlTestcontainer _msSqlContainer;
  private readonly PostgreSqlTestcontainer _postgresContainer;
  private readonly OracleTestcontainer _oracleContainer;

  private readonly Lazy<SqlConnection> _sqlServerConnectionLazy;
  private readonly Lazy<NpgsqlConnection> _postgresConnectionLazy;
  private readonly Lazy<OracleConnection> _oracleConnectionLazy;

  private readonly TestLocalData _localData;
  private readonly RepositoryFactory _repositoryFactory;

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

    _oracleContainer = new TestcontainersBuilder<OracleTestcontainer>()
      .WithDatabase(new OracleTestcontainerConfiguration
      {
        Password = "Password1"
      })
      .Build();

    _sqlServerConnectionLazy = new(() => new SqlConnection(SqlServerConnectionString));
    _postgresConnectionLazy = new(() => new NpgsqlConnection(PostgresConnectionString));
    _oracleConnectionLazy = new(() => new OracleConnection(OracleConnectionString));

    _localData = new(this);
    _repositoryFactory = new(this);
  }

  public TestDbContext DbContext => _localData.DbContext;

  public string SqlServerConnectionString => _msSqlContainer.ConnectionString + "TrustServerCertificate=True;";

  public string PostgresConnectionString => _postgresContainer.ConnectionString;

  public string OracleConnectionString => _oracleContainer.ConnectionString;

  public SqlConnection SqlServerConnection => _sqlServerConnectionLazy.Value;

  public NpgsqlConnection PostgresConnection => _postgresConnectionLazy.Value;

  public OracleConnection OracleConnection => _oracleConnectionLazy.Value;

  public Repository<TEntity, TKey> CreateRepository<TEntity, TKey>(
    RepositoryDependencies dependencies,
    IEnumerable<object>? seed = null)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    _localData.InitializeContext(dependencies.Provider, seed);

    return _repositoryFactory.CreateRepository<TEntity, TKey>(dependencies);
  }

  public void Setup(ITestOutputHelper output)
  {
    _localData.Setup(output);
  }

  public void Reset()
  {
    _localData.Reset();
  }

  async Task IAsyncLifetime.InitializeAsync()
  {
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
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
    await _oracleContainer.StartAsync();

    _localData.EnsureCreated();
  }

  async Task IAsyncLifetime.DisposeAsync()
  {
    _sqlServerConnectionLazy.Value.Dispose();
    _postgresConnectionLazy.Value.Dispose();
    _oracleConnectionLazy.Value.Dispose();

    await CleanUpContainerAsync(_msSqlContainer);
    await CleanUpContainerAsync(_postgresContainer);
    await CleanUpContainerAsync(_oracleContainer);

    static async Task CleanUpContainerAsync(TestcontainersContainer container)
    {
      await container.StopAsync();
      await container.CleanUpAsync();
      await container.DisposeAsync();
    }
  }
}
