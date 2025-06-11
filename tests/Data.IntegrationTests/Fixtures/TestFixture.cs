using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.Data.AdoNet.MySQL;
using CodeArchitects.Platform.Data.AdoNet.Oracle;
using CodeArchitects.Platform.Data.AdoNet.PostgreSQL;
using CodeArchitects.Platform.Data.AdoNet.SQLServer;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Concurrency;
using Docker.DotNet;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;
using Testcontainers.MariaDb;
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
  private readonly MariaDbContainer _mariaDbContainer;

  private IServiceProvider _sqlServerServices = default!;
  private IServiceProvider _postgresServices = default!;
  private IServiceProvider _oracleServices = default!;
  private IServiceProvider _mariaDbServices = default!;

  private bool _isExecuting;
  private readonly XunitLoggerFactory _loggerFactory;

  public TestFixture()
  {
    _msSqlContainer = new MsSqlBuilder().Build();
    _postgresContainer = new PostgreSqlBuilder().Build();
    _oracleContainer = new OracleBuilder().Build();
    _mariaDbContainer = new MariaDbBuilder().Build();

    _loggerFactory = new XunitLoggerFactory(this);
  }

  public ITestOutputHelper? Output { get; private set; }

  private string SqlServerConnectionString => _msSqlContainer.GetConnectionString();

  private string PostgresConnectionString => _postgresContainer.GetConnectionString();

  private string OracleConnectionString => _oracleContainer.GetConnectionString();

  private string MariaDbConnectionString => _mariaDbContainer.GetConnectionString();

  internal DatabaseFixture CreateDbFixture(RepositoryDependencies dependencies)
  {
    IServiceProvider services = GetProviderServices(dependencies.Provider);
    return DatabaseFixture.Create(services, dependencies);
  }

  internal TestScope CreateScope(RepositoryDependencies dependencies)
  {
    IServiceProvider services = GetProviderServices(dependencies.Provider);
    return TestScope.Create(services, dependencies);
  }

  public void Setup(ITestOutputHelper output)
  {
    if (_isExecuting)
      throw new InvalidOperationException("Another test is executing.");

    _isExecuting = true;
    Output = output;
  }

  public void Reset()
  {
    if (!_isExecuting)
      throw new InvalidOperationException("Nothing to reset.");

    _isExecuting = false;
    Output = null;
  }

  public IServiceProvider GetProviderServices(DbProvider dbProvider)
  {
    return dbProvider switch
    {
      DbProvider.SqlServer => _sqlServerServices,
      DbProvider.Postgres => _postgresServices,
      DbProvider.Oracle => _oracleServices,
      DbProvider.MariaDb => _mariaDbServices,
      _ => throw Errors.Unreachable
    };
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
    await _mariaDbContainer.StartAsync();

    _sqlServerServices = CreateServiceProvider(
      options => options.UseSqlServer(SqlServerConnectionString),
      options => options.UseProvider<SQLServerProvider>(sqlServer => sqlServer.UseConnection(SqlServerConnectionString)));

    _postgresServices = CreateServiceProvider(
      options => options.UseNpgsql(PostgresConnectionString),
      options => options.UseProvider<PostgreSQLProvider>(postgres => postgres.UseConnection(PostgresConnectionString)));

#if NET7_0
    _oracleServices = CreateServiceProvider(
      options => options.UseOracle(OracleConnectionString),
      options => options.UseProvider<OracleProvider>(oracle => oracle.UseConnection(OracleConnectionString)));
#elif NET8_0_OR_GREATER
    _oracleServices = CreateServiceProvider(
      options => options.UseOracle(OracleConnectionString, b =>
        b.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19)),
      options => options.UseProvider<OracleProvider>(oracle => oracle.UseConnection(OracleConnectionString)));
#endif

    _mariaDbServices = CreateServiceProvider(
      options => options.UseMySql(MariaDbConnectionString, new MariaDbServerVersion("10.10.0")),
      options => options.UseProvider<MySQLProvider>(mySql => mySql.UseConnection(MariaDbConnectionString)));
  }

  private IServiceProvider CreateServiceProvider(
    Func<DbContextOptionsBuilder, DbContextOptionsBuilder> useEfCoreProvider,
    Func<IAdoNetConfigurationBuilder, IAdoNetConfigurationBuilderWithProvider> useAdoNetProvider)
  {
    ServiceCollection serviceCollection = new();

    serviceCollection.AddDbContext<TestDbContext>(options => useEfCoreProvider(options)
      .EnableSensitiveDataLogging()
      .UseLoggerFactory(_loggerFactory)
      .ConfigureWarnings(warnings =>
        warnings.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning)
      )
      .UseCaep(caep => caep
        .UseOptimisticConcurrency()));

    serviceCollection.AddData<TestDbContext>();

    serviceCollection.AddData(options => useAdoNetProvider(options).UseModel<TestModelConfiguration>());

    IServiceProvider services = serviceCollection.BuildServiceProvider();
    using IServiceScope scope = services.CreateScope();

    bool created = scope.ServiceProvider
      .GetRequiredService<TestDbContext>()
      .Database
      .EnsureCreated();

    created.Should().BeTrue();

    return services;
  }

  async Task IAsyncLifetime.DisposeAsync()
  {
    Environment.SetEnvironmentVariable("EFCORE_ISRUNTIME", null);

    await CleanUpContainerAsync(_msSqlContainer);
    await CleanUpContainerAsync(_postgresContainer);
    await CleanUpContainerAsync(_oracleContainer);
    await CleanUpContainerAsync(_mariaDbContainer);

    static async Task CleanUpContainerAsync(DockerContainer container)
    {
      await container.StopAsync();
      await container.DisposeAsync();
    }
  }
}
