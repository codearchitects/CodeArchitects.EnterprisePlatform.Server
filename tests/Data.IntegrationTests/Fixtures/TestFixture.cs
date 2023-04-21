using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.Data.AdoNet;
using CodeArchitects.Platform.Data.EntityFrameworkCore;
using CodeArchitects.Platform.Data.Tracking;
using Docker.DotNet;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;
using Testcontainers.MsSql;
using Testcontainers.Oracle;
using Testcontainers.PostgreSql;
using Xunit.Abstractions;

namespace CodeArchitects.Platform.Data.Fixtures;

public class TestFixture : IAsyncLifetime
{
  private readonly MsSqlContainer _msSqlContainer;
  private readonly PostgreSqlContainer _postgresContainer;
  private readonly OracleContainer _oracleContainer;

  private readonly TestLocalData _localData;

  public TestFixture()
  {
    _msSqlContainer = new MsSqlBuilder().Build();
    _postgresContainer = new PostgreSqlBuilder().Build();
    _oracleContainer = new OracleBuilder().Build();

    _localData = new(this);
  }

  public TestDbContext DbContext => _localData.DbContext;

  public ITrackingContext TrackingContext => _localData.Services.GetRequiredService<ITrackingContext>();

  public string SqlServerConnectionString => _msSqlContainer.GetConnectionString();

  public string PostgresConnectionString => _postgresContainer.GetConnectionString();

  public string OracleConnectionString => _oracleContainer.GetConnectionString();

  public Repository<TEntity, TKey> CreateRepository<TEntity, TKey>(
    RepositoryDependencies dependencies,
    Action<ISeeder>? seedingAction = null,
    RepositoryImplementation seedImplementation = default)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (seedImplementation == default)
    {
      seedImplementation = dependencies.Implementation;
    }

    _localData.InitializeServices(dependencies.Provider, new TestDataSeed(seedingAction ?? (seeder => { })), seedImplementation);

    return dependencies.Implementation switch
    {
      RepositoryImplementation.AdoNet => new AdoNetRepository<TEntity, TKey>(_localData.Services.GetRequiredService<AdoNet.IDataContext>()),
      RepositoryImplementation.EFCore => new EFCoreRepository<TEntity, TKey>(_localData.Services.GetRequiredService<EntityFrameworkCore.IDataContext>()),
      _                               => throw Errors.Unreachable
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
    await CleanUpContainerAsync(_msSqlContainer);
    await CleanUpContainerAsync(_postgresContainer);
    await CleanUpContainerAsync(_oracleContainer);

    static async Task CleanUpContainerAsync(DockerContainer container)
    {
      await container.StopAsync();
      await container.DisposeAsync();
    }
  }
}
