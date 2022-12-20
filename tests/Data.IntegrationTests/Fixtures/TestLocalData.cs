using CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;
using CodeArchitects.Platform.Data.Fixtures.Model;
using Microsoft.EntityFrameworkCore;
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

  private readonly Lazy<DbContextOptions> _sqlServerOptionsLazy;
  private readonly Lazy<DbContextOptions> _postgresOptionsLazy;
  private readonly Lazy<DbContextOptions> _oracleOptionsLazy;

  public TestLocalData(TestFixture fixture)
  {
    _sqlServerOptionsLazy = new(() => new DbContextOptionsBuilder()
      .UseSqlServer(fixture.SqlServerConnectionString)
      .EnableSensitiveDataLogging()
      .UseLoggerFactory(new XunitLoggerFactory(this))
      .UseCaep()
      .Options);

    _postgresOptionsLazy = new(() => new DbContextOptionsBuilder()
      .UseNpgsql(fixture.PostgresConnectionString)
      .EnableSensitiveDataLogging()
      .UseLoggerFactory(new XunitLoggerFactory(this))
      .UseCaep()
      .Options);

    _oracleOptionsLazy = new(() => new DbContextOptionsBuilder()
      .UseOracle(fixture.OracleConnectionString)
      .EnableSensitiveDataLogging()
      .UseLoggerFactory(new XunitLoggerFactory(this))
      .UseCaep()
      .Options);
  }

  private bool _isInitialized;
  private TestDbContext? _dbContext;
  private DbProvider _provider = s_defaultProvider;

  public TestDbContext DbContext => _dbContext ?? throw new InvalidOperationException("No context was initialized.");

  public ITestOutputHelper? Output { get; private set; }

  public void EnsureCreated()
  {
    new TestDbContext(_sqlServerOptionsLazy.Value).Database.EnsureCreated();
    new TestDbContext(_postgresOptionsLazy.Value).Database.EnsureCreated();
    new TestDbContext(_oracleOptionsLazy.Value).Database.EnsureCreated();
  }

  public void Setup(ITestOutputHelper output)
  {
    if (_isInitialized)
      throw new InvalidOperationException("Another test is already executing.");
    _isInitialized = true;

    Output = output;
  }

  public void InitializeContext(DbProvider provider, IEnumerable<object>? seed)
  {
    if (!_isInitialized)
      throw new InvalidOperationException("Local data was not initialized.");
    if (_dbContext is not null)
      throw new InvalidOperationException("Another context was already initialized.");

    _provider = provider;
    DbContextOptions options = provider switch
    {
      DbProvider.SqlServer => _sqlServerOptionsLazy.Value,
      DbProvider.Postgres  => _postgresOptionsLazy.Value,
      DbProvider.Oracle    => _oracleOptionsLazy.Value,
      _                    => throw Errors.Unreacheable
    };
    _dbContext = new(options);

    if (seed is not null)
    {
      _dbContext.AddRange(seed);
      _dbContext.SaveChanges();
    }
  }

  public void Reset()
  {
    if (!_isInitialized)
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
    DbContext.Dispose();
    _dbContext = null;
    _provider = s_defaultProvider;

    _isInitialized = false;
  }
}
