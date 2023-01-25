using CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.Data.AdoNet;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Oracle;
using CodeArchitects.Platform.Data.AdoNet.PostgreSQL;
using CodeArchitects.Platform.Data.AdoNet.SQLServer;
using CodeArchitects.Platform.Data.EntityFrameworkCore;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;
using CodeArchitects.Platform.Data.Fixtures.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using Xunit.Abstractions;

namespace CodeArchitects.Platform.Data.Fixtures;

public class TestLocalData
{
  private static readonly DbProvider s_defaultProvider = (DbProvider)(-1);
  private static readonly string[] s_entityNames = new[]
  {
    nameof(Address),
    nameof(CartItem),
    nameof(Cart),
    nameof(Category),
    nameof(Person),
    nameof(Product),
    nameof(SerialEntity),
    nameof(Typology),
    nameof(CustomerClaim),
    nameof(Customer)
  };

  private readonly TestFixture _fixture;
  
  private IServiceProvider _sqlServerServices = default!;
  private IServiceProvider _postgresServices = default!;
  private IServiceProvider _oracleServices = default!;

  private IServiceScope? _scope;
  private IServiceProvider? _services;
  private TestDbContext? _dbContext;

  public TestLocalData(TestFixture fixture)
  {
    _fixture = fixture;
  }

  private bool _isExecuting;
  private DbProvider _provider = s_defaultProvider;

  public IServiceProvider Services => _services ?? throw new InvalidOperationException("Services were not initialized.");

  public TestDbContext DbContext => _dbContext ?? throw new InvalidOperationException("Services were not initialized.");

  public ITestOutputHelper? Output { get; private set; }

  public void Initialize()
  {
    ServiceCollection sqlServerServiceCollection = new();
    ServiceCollection postgresServiceCollection = new();
    ServiceCollection oracleServiceCollection = new();

    sqlServerServiceCollection.AddDbContext<TestDbContext>(options => options
      .UseSqlServer(_fixture.SqlServerConnectionString)
      .EnableSensitiveDataLogging()
      .UseLoggerFactory(new XunitLoggerFactory(this))
      .UseCaep());

    sqlServerServiceCollection.AddData<TestDbContext>();

    sqlServerServiceCollection.AddData(options => options
      .UseProvider<SQLServerProvider>(provider => provider.UseConnection(_fixture.SqlServerConnectionString))
      .UseModel<TestModelConfiguration>());

    postgresServiceCollection.AddDbContext<TestDbContext>(options => options
      .UseNpgsql(_fixture.PostgresConnectionString)
      .EnableSensitiveDataLogging()
      .UseLoggerFactory(new XunitLoggerFactory(this))
      .UseCaep());

    postgresServiceCollection.AddData<TestDbContext>();

    postgresServiceCollection.AddData(options => options
      .UseProvider<PostgreSQLProvider>(provider => provider.UseConnection(_fixture.PostgresConnectionString))
      .UseModel<TestModelConfiguration>());

    oracleServiceCollection.AddDbContext<TestDbContext>(options => options
      .UseOracle(_fixture.OracleConnectionString)
      .EnableSensitiveDataLogging()
      .UseLoggerFactory(new XunitLoggerFactory(this))
      .UseCaep());

    oracleServiceCollection.AddData<TestDbContext>();

    oracleServiceCollection.AddData(options => options
      .UseProvider<OracleProvider>(provider => provider.UseConnection(_fixture.OracleConnectionString))
      .UseModel<TestModelConfiguration>());

    _sqlServerServices = sqlServerServiceCollection.BuildServiceProvider();
    _postgresServices = postgresServiceCollection.BuildServiceProvider();
    _oracleServices = oracleServiceCollection.BuildServiceProvider();

    using IServiceScope sqlServerScope = _sqlServerServices.CreateScope();
    using IServiceScope postgresScope = _postgresServices.CreateScope();
    using IServiceScope oracleScope = _oracleServices.CreateScope();

    TestDbContext dbContext;

    dbContext = sqlServerScope.ServiceProvider.GetRequiredService<TestDbContext>();
    dbContext.Database.EnsureCreated();

    dbContext = postgresScope.ServiceProvider.GetRequiredService<TestDbContext>();
    dbContext.Database.EnsureCreated();

    dbContext = oracleScope.ServiceProvider.GetRequiredService<TestDbContext>();
    dbContext.Database.EnsureCreated();
  }

  public void Setup(ITestOutputHelper output)
  {
    if (_isExecuting)
      throw new InvalidOperationException("Another test is already executing.");

    _isExecuting = true;
    Output = output;
  }

  public void InitializeServices(DbProvider provider, DataSeed seed, RepositoryImplementation seedImplementation)
  {
    if (!_isExecuting)
      throw new InvalidOperationException("Local data was not set up.");

    _provider = provider;
    IServiceProvider services = provider switch
    {
      DbProvider.SqlServer => _sqlServerServices,
      DbProvider.Postgres  => _postgresServices,
      DbProvider.Oracle    => _oracleServices,
      _                    => throw Errors.Unreacheable
    };
    _scope = services.CreateScope();
    _services = _scope.ServiceProvider;
    _dbContext = _services.GetRequiredService<TestDbContext>();

    switch (seedImplementation)
    {
      case RepositoryImplementation.EFCore:
        _dbContext.Seed(seed);
        break;
      case RepositoryImplementation.AdoNet:
        DatabaseProvider databaseProvider = provider switch
        {
          DbProvider.SqlServer => services.GetRequiredService<SQLServerProvider>(),
          DbProvider.Postgres  => services.GetRequiredService<PostgreSQLProvider>(),
          DbProvider.Oracle    => services.GetRequiredService<OracleProvider>(),
          _                    => throw Errors.Unreacheable
        };

        databaseProvider.ApplySeed(services, seed);
        break;
    }
  }

  public void Reset()
  {
    if (!_isExecuting)
      throw new InvalidOperationException("No data to reset.");

    DbConnection connection = DbContext.Database.GetDbConnection();
    connection.Open();
    using (DbCommand command = connection.CreateCommand())
    {
      char escapeLeft = _provider is DbProvider.SqlServer ? '[' : '"';
      char escapeRight = _provider is DbProvider.SqlServer ? ']' : '"';
      foreach (string entityName in s_entityNames)
      {
        command.CommandText = $"DELETE FROM {escapeLeft}{entityName}{escapeRight}";
        command.ExecuteNonQuery();
      }
    }
    connection.Close();

    Output = null;
    _scope?.Dispose();
    _provider = s_defaultProvider;

    _isExecuting = false;
  }
}
