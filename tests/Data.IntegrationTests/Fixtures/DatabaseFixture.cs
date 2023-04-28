using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.Data.AdoNet;
using CodeArchitects.Platform.Data.AdoNet.MySQL;
using CodeArchitects.Platform.Data.AdoNet.Oracle;
using CodeArchitects.Platform.Data.AdoNet.PostgreSQL;
using CodeArchitects.Platform.Data.AdoNet.SQLServer;
using CodeArchitects.Platform.Data.EntityFrameworkCore;
using CodeArchitects.Platform.Data.Fixtures.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.Fixtures;

internal sealed class DatabaseFixture : IDisposable
{
  private static readonly string[] s_entityNames = new[]
  {
    nameof(Address),
    nameof(CartItem),
    nameof(Cart),
    nameof(Category),
    nameof(ConcurrencyEntity),
    nameof(Person),
    nameof(Product),
    nameof(SerialEntity),
    nameof(Typology),
    nameof(CustomerClaim),
    nameof(Customer),
    nameof(ShippingAddress)
  };

  private readonly IServiceScope _scope;
  private readonly DbProvider _provider;
  private readonly RepositoryImplementation _implementation;

  public DatabaseFixture(IServiceScope scope, DbProvider provider, RepositoryImplementation implementation)
  {
    _scope = scope;
    _provider = provider;
    _implementation = implementation;
  }

  public void Seed(Action<ISeeder> seedingAction, RepositoryImplementation seedImplementation = RepositoryImplementation.Default)
  {
    IServiceProvider services = _scope.ServiceProvider;
    TestDataSeed seed = new(seedingAction);
    if (seedImplementation is RepositoryImplementation.Default)
    {
      seedImplementation = _implementation;
    }

    switch (seedImplementation)
    {
      case RepositoryImplementation.EFCore:
        TestDbContext dbContext = services.GetRequiredService<TestDbContext>();
        dbContext.Seed(seed);
        break;
      case RepositoryImplementation.AdoNet:
        DatabaseProvider databaseProvider = _provider switch
        {
          DbProvider.SqlServer => services.GetRequiredService<SQLServerProvider>(),
          DbProvider.Postgres  => services.GetRequiredService<PostgreSQLProvider>(),
          DbProvider.Oracle    => services.GetRequiredService<OracleProvider>(),
          DbProvider.MariaDb   => services.GetRequiredService<MySQLProvider>(),
          _                    => throw Errors.Unreachable
        };

        databaseProvider.ApplySeed(services, seed);
        break;
    }
  }

  public void Dispose()
  {
    DbConnection connection = _scope.ServiceProvider.GetRequiredService<TestDbContext>().Database.GetDbConnection();
    connection.Open();
    using (DbCommand command = connection.CreateCommand())
    {
      foreach (string entityName in s_entityNames)
      {
        if (entityName is "Person" && _provider is DbProvider.MariaDb)
        {
          command.CommandText = $"UPDATE {Escape(entityName)} SET {Escape("PartnerId")} = NULL WHERE {Escape("PartnerId")} IS NOT NULL";
          command.ExecuteNonQuery();
        }

        command.CommandText = $"DELETE FROM {Escape(entityName)}";
        command.ExecuteNonQuery();
      }
    }
    connection.Close();

    _scope.Dispose();
  }

  private string Escape(string name)
  {
    char escapeLeft = _provider switch
    {
      DbProvider.SqlServer => '[',
      DbProvider.MariaDb   => '`',
      _                    => '"'
    };
    char escapeRight = _provider switch
    {
      DbProvider.SqlServer => ']',
      DbProvider.MariaDb   => '`',
      _                    => '"'
    };

    return $"{escapeLeft}{name}{escapeRight}";
  }

  public static DatabaseFixture Create(IServiceProvider services, RepositoryDependencies dependencies)
  {
    IServiceScope scope = services.CreateScope();
    return new(scope, dependencies.Provider, dependencies.Implementation);
  }
}
