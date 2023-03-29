using CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.Data.AdoNet;
using CodeArchitects.Platform.Data.EntityFrameworkCore;
using CodeArchitects.Platform.Data.Tracking;
using Docker.DotNet;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
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
  }

  public TestDbContext DbContext => _localData.DbContext;

  public ITrackingContext TrackingContext => _localData.Services.GetRequiredService<ITrackingContext>();

  public string SqlServerConnectionString => _msSqlContainer.ConnectionString + "TrustServerCertificate=True;";

  public string PostgresConnectionString => _postgresContainer.ConnectionString;

  public string OracleConnectionString => _oracleContainer.ConnectionString;

  public SqlConnection SqlServerConnection => _sqlServerConnectionLazy.Value;

  public NpgsqlConnection PostgresConnection => _postgresConnectionLazy.Value;

  public OracleConnection OracleConnection => _oracleConnectionLazy.Value;

  public Repository<TEntity, TKey> CreateRepository<TEntity, TKey>(
    RepositoryDependencies dependencies,
    Action<ISeeder>? seedingAction = null,
    RepositoryImplementation seedImplementation = ((RepositoryImplementation)(-1)))
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (seedImplementation == ((RepositoryImplementation)(-1)))
    {
      seedImplementation = dependencies.Implementation;
    }

    _localData.InitializeServices(dependencies.Provider, new TestDataSeed(seedingAction ?? (seeder => { })), seedImplementation);

    return dependencies.Implementation switch
    {
      RepositoryImplementation.AdoNet => new AdoNetRepository<TEntity, TKey>(_localData.Services.GetRequiredService<AdoNet.IDataContext>()),
      RepositoryImplementation.EFCore => new EFCoreRepository<TEntity, TKey>(_localData.Services.GetRequiredService<EntityFrameworkCore.IDataContext>()),
      _                               => throw Errors.Unreacheable
    };
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
    Environment.SetEnvironmentVariable("EFCORE_ISRUNTIME", "true");

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

    _localData.Initialize();
  }

  async Task IAsyncLifetime.DisposeAsync()
  {
    Environment.SetEnvironmentVariable("EFCORE_ISRUNTIME", null);

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
